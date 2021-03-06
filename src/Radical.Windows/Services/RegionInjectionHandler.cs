﻿using Radical.Windows.ComponentModel;
using System;
using System.Collections.Generic;
using System.Windows;

namespace Radical.Windows.Services
{
    class RegionInjectionHandler : IRegionInjectionHandler
    {
        readonly Dictionary<string, List<Type>> viewsInterestedInRegions = new Dictionary<string, List<Type>>();

        /// <summary>
        /// Initializes a new instance of the <see cref="RegionInjectionHandler"/> class.
        /// </summary>
        public RegionInjectionHandler()
        {
            Inject = ( viewFactory, region ) =>
            {
                if ( region.AsyncLoadDelay > 0 )
                {
                    Wait.For( TimeSpan.FromMilliseconds( region.AsyncLoadDelay ) )
                        .AndThen( () => 
                        {
                            var view = viewFactory();
                            InjectViewIntoRegion( view, region );
                        } );
                }
                else
                {
                    var view = viewFactory();
                    InjectViewIntoRegion( view, region );
                }
            };
        }

        void InjectViewIntoRegion( DependencyObject view, IRegion region ) 
        {
            if ( region is IContentRegion )
            {
                ( ( IContentRegion )region ).Content = view;
            }
            else if ( region is IElementsRegion )
            {
                ( ( IElementsRegion )region ).Add( view );
            }
        }

        /// <summary>
        /// Gets the views interested in the region identified by the given name.
        /// </summary>
        /// <param name="regionName">Name of the region.</param>
        /// <returns>
        /// A list of view types.
        /// </returns>
        public IEnumerable<Type> GetViewsInterestedIn( string regionName )
        {
            if( viewsInterestedInRegions.ContainsKey( regionName ) )
            {
                var views = viewsInterestedInRegions[ regionName ];
                return views.AsReadOnly();
            }

            return new Type[ 0 ];
        }

        /// <summary>
        /// Registers the view as interested in the region that has the given region name.
        /// </summary>
        /// <param name="regionName">Name of the region.</param>
        /// <param name="viewType">Type of the view.</param>
        public void RegisterViewAsInterestedIn( string regionName, Type viewType )
        {
            RegisterViewsAsInterestedIn( regionName, new[] { viewType } );
        }

        /// <summary>
        /// Registers the views as interested in the region that has the given region name.
        /// </summary>
        /// <param name="regionName">Name of the region.</param>
        /// <param name="views">The views.</param>
        public void RegisterViewsAsInterestedIn( string regionName, IEnumerable<Type> views )
        {
            if( viewsInterestedInRegions.ContainsKey( regionName ) )
            {
                viewsInterestedInRegions[ regionName ].AddRange( views );
            }
            else
            {
                viewsInterestedInRegions.Add( regionName, new List<Type>( views ) );
            }
        }

        /// <summary>
        /// Gets or sets the inject handler used to inject the given view into the given region.
        /// </summary>
        /// <value>
        /// The inject handler.
        /// </value>
        [IgnorePropertyInjectionAttribue]
        public Action<Func<System.Windows.DependencyObject>, ComponentModel.IRegion> Inject
        {
            get;
            set;
        }
    }
}
