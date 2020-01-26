﻿using Radical.Validation;
using Radical.Windows.ComponentModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;

namespace Radical.Windows.Presentation.Regions
{
    /// <summary>
    /// The region service is responsible for managing regions, shell regions and
    /// modules specific regions.
    /// </summary>
    public class RegionService : IRegionService
    {
        /// <summary>
        /// Gets or sets the conventions handler.
        /// </summary>
        /// <value>
        /// The conventions handler.
        /// </value>
        public static IConventionsHandler Conventions { get; set; }

        /// <summary>
        /// Gets or sets the current service.
        /// </summary>
        /// <value>
        /// The current service.
        /// </value>
        public static IRegionService CurrentService
        {
            get;
            set;
        }
        /// <summary>
        /// The region attached property.
        /// </summary>
        public static readonly DependencyProperty RegionProperty = DependencyProperty.RegisterAttached(
                                      "Region",
                                      typeof( IRegion ),
                                      typeof( RegionService ),
                                      new PropertyMetadata( null, OnRegionPropertyChanged ) );

        /// <summary>
        /// Gets the region.
        /// </summary>
        /// <param name="control">The control.</param>
        /// <returns>The region.</returns>
        public static IRegion GetRegion( FrameworkElement control )
        {
            return ( IRegion )control.GetValue( RegionProperty );
        }

        /// <summary>
        /// Sets the region.
        /// </summary>
        /// <param name="control">The control.</param>
        /// <param name="value">The value.</param>
        public static void SetRegion( FrameworkElement control, IRegion value )
        {
            control.SetValue( RegionProperty, value );
        }

        static void OnRegionPropertyChanged( DependencyObject d, DependencyPropertyChangedEventArgs args )
        {
            if ( !DesignerProperties.GetIsInDesignMode( d ) )
            {
                var region = ( IRegion )args.NewValue;

                var service = CurrentService;
                IRegionManager manager = null;

                if ( service.HoldsRegionManager( region.HostingView ) )
                {
                    manager = service.GetRegionManager( region.HostingView );
                }
                else
                {
                    manager = service.RegisterRegionManager( region.HostingView );
                }

                manager.RegisterRegion( region );
            }
        }

        readonly IRegionManagerFactory regionManagerFactory;
        readonly IConventionsHandler conventions;
        readonly IDictionary<DependencyObject, IRegionManager> regionManagers = new Dictionary<DependencyObject, IRegionManager>();
        readonly HashSet<DependencyObject> closableObjects = new HashSet<DependencyObject>();

        /// <summary>
        /// Initializes a new instance of the <see cref="RegionService"/> class.
        /// </summary>
        /// <param name="regionManagerFactory">The region manager factory.</param>
        /// <param name="conventions">The conventions.</param>
        public RegionService( IRegionManagerFactory regionManagerFactory, IConventionsHandler conventions )
        {
            Ensure.That( regionManagerFactory ).Named( () => regionManagerFactory ).IsNotNull();
            Ensure.That( conventions ).Named( () => conventions ).IsNotNull();

            this.regionManagerFactory = regionManagerFactory;
            this.conventions = conventions;
        }

        /// <summary>
        /// Determines if this region service has knowledge of a region manager owned
        /// by the supplied owner (tipically an IView).
        /// </summary>
        /// <param name="owner">The owner.</param>
        /// <returns>
        /// True if this region service has a reference to a region manager owned by the supplied owner, otherwise false.
        /// </returns>
        public bool HoldsRegionManager( DependencyObject owner )
        {
            return regionManagers.ContainsKey( owner );
        }

        /// <summary>
        /// Gets the region manager owned by the supplied view.
        /// </summary>
        /// <param name="owner">The owner.</param>
        /// <returns>
        /// A reference to the requested region manager.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">An ArgumentOutOfRangeException is raised if this service has no knowledge of a region manager owned by the supplied owner. Use HoldsRegionManager() to test.</exception>
        public IRegionManager GetRegionManager( DependencyObject owner )
        {
            if ( !regionManagers.ContainsKey( owner ) )
            {
                throw new ArgumentOutOfRangeException( "owner", "Cannot find any region manager owned by the supplied view." );
            }

            return regionManagers[ owner ];
        }

        /// <summary>
        /// Gets a known region manager. A known region manager is a region
        /// manager associated with a view type and not with a view instance.
        /// Tipically this region manager is a static region manager that exists
        /// for all the application life-cycle. A good sample of a known region
        /// manager is the Shell RegionManager that exists always.
        /// </summary>
        /// <typeparam name="TView">The type of the view.</typeparam>
        /// <returns>The requested region manager</returns>
        public IRegionManager GetKnownRegionManager<TView>() where TView : DependencyObject
        {
            return regionManagers.Where( kvp => typeof( TView ).IsAssignableFrom( kvp.Key.GetType() ) )
                .Select( kvp => kvp.Value )
                .FirstOrDefault();
        }

        /// <summary>
        /// Finds a region manager given a custom search logic.
        /// </summary>
        /// <param name="filter">A predicate executed for all the registered region managers.</param>
        /// <returns>The found region manager or null.</returns>
        public IRegionManager FindRegionManager( Func<DependencyObject, IRegionManager, bool> filter )
        {
            return regionManagers.Where( kvp => filter( kvp.Key, kvp.Value ) )
                .Select( v => v.Value )
                .SingleOrDefault();
        }

        /// <summary>
        /// Registers a new region manager for the given owner.
        /// </summary>
        /// <param name="owner">The owner.</param>
        /// <returns>The newly registered region manager.</returns>
        /// <exception cref="NotSupportedException">A NotSupportedException is raised if this service has already registered a region manager for the supplied owner. Use HoldsRegionManager() to test.</exception>
        public IRegionManager RegisterRegionManager( DependencyObject owner )
        {
            if ( HoldsRegionManager( owner ) )
            {
                throw new NotSupportedException();
            }

            var manager = regionManagerFactory.Create();
            regionManagers.Add( owner, manager );

            /*
             * L'inghippo è:
             * 
             * Qualcuno ci chiede di registrare un nuovo region manager, quello
             * che dobbiamo fare è avere la possibilità di deregistrare quel region 
             * manager, quindi:
             *    - facciamo il walking del visual tree alla ricerca di qualcosa di "chiudibile"
             *        * o IClosableView
             *        * o Window
             *    - se troviamo uno dei 2;
             *    - aggiungiamo questa Window/IClosableView alla lista di quelle aperte;
             *    - ci agganciamo all'evento Closed;
             *    - quando la IClosableView/Window è stata chiusa dobbiamo scorrere il VisualTree
             *      dall'alto verso il basso e cercare controlli che siano View, se ne troviamo
             *      vediamo se per quella View abbiamo registrato dei region manager in caso
             *      affermativo de-registriamo.
             */
            void closedCallback(DependencyObject d)
            {
                if (conventions.ShouldUnregisterRegionManagerOfView(d))
                {
                    UnregisterRegionManagers(d, UnregisterBehavior.WholeLogicalTreeChain);
                }
            }

            var closableHost = conventions.TryHookClosedEventOfHostOf( owner, closedCallback );
            if ( closableHost != null )
            {
                closableObjects.Add( closableHost );
            }

            return manager;
        }

        void UnregisterRegionManagers( DependencyObject view, UnregisterBehavior behavior )
        {
            if ( view != null )
            {
                if ( HoldsRegionManager( view ) )
                {
                    var manager = regionManagers[ view ];
                    manager.Shutdown();

                    regionManagers.Remove( view );
                }

                if ( closableObjects.Contains( view ) )
                {
                    closableObjects.Remove( view );
                }

                if ( behavior == UnregisterBehavior.WholeLogicalTreeChain )
                {
                    var children = LogicalTreeHelper.GetChildren( view );
                    foreach ( var child in children )
                    {
                        UnregisterRegionManagers( child as DependencyObject, behavior );
                    }
                }
            }
        }

        /// <summary>
        /// Unregisters the region manager owned by the supplied owner.
        /// </summary>
        /// <param name="owner">The owner of the region manager to unregister.</param>
        /// <exception cref="NotSupportedException">A NotSupportedException is raised if this service has no region manager registered for the supplied owner. Use HoldsRegionManager() to test.</exception>
        public void UnregisterRegionManager( DependencyObject owner )
        {
            UnregisterRegionManager( owner, UnregisterBehavior.Default );
        }

        /// <summary>
        /// Unregisters the region manager owned by the supplied owner.
        /// </summary>
        /// <param name="owner">The owner of the region manager to unregister.</param>
        /// <param name="behavior">How to manage region manager found in child/nested views.</param>
        /// <exception cref="NotSupportedException">A NotSupportedException is raised if this service has no region manager registered for the supplied owner. Use HoldsRegionManager() to test.</exception>
        public void UnregisterRegionManager( DependencyObject owner, UnregisterBehavior behavior )
        {
            if ( !HoldsRegionManager( owner ) )
            {
                throw new NotSupportedException();
            }

            UnregisterRegionManagers( owner, behavior );
        }
    }
}
