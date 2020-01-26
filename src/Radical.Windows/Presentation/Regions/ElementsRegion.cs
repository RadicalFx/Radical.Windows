using Radical.Conversions;
using Radical.Validation;
using Radical.Windows.ComponentModel;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;

namespace Radical.Windows.Presentation.Regions
{
    /// <summary>
    /// A base abstract implementation of the <see cref="IElementsRegion"/>.
    /// </summary>
    /// <typeparam name="T">The type of the element that hosts this region.</typeparam>
    public abstract class ElementsRegion<T> :
        Region<T>,
        IElementsRegion
        where T : FrameworkElement
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ElementsRegion&lt;T&gt;"/> class.
        /// </summary>
        protected ElementsRegion()
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ElementsRegion&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        protected ElementsRegion(string name )
            : base( name )
        {

        }

        /// <summary>
        /// Closes this region, the close process is invoked by the host at close time.
        /// </summary>
        public override void Shutdown()
        {
            if ( RegionService.Conventions == null )
            {
                //BUG: se è null l'app sta facendo shutdown.
                return;
            }

            views.ForEach( v =>
            {
                NotifyClosedAndEnsureRelease( v );
            } );
            views.Clear();
        }

        readonly List<DependencyObject> views = new List<DependencyObject>();

        /// <summary>
        /// Adds the specified view.
        /// </summary>
        /// <param name="view">The view.</param>
        public void Add( DependencyObject view )
        {
            Ensure.That(view).Named(nameof(view)).IsNotNull();

            OnAdd( view );
            views.Add( view );
            OnAdded( view );
        }

        /// <summary>
        /// Called before the add operation.
        /// </summary>
        /// <param name="view">The view.</param>
        protected abstract void OnAdd( DependencyObject view );

        /// <summary>
        /// Called after the add operation.
        /// </summary>
        /// <param name="view">The view.</param>
        protected virtual void OnAdded( DependencyObject view ) 
        {

        }

        /// <summary>
        /// Removes the specified view.
        /// </summary>
        /// <param name="view">The view.</param>
        public void Remove( DependencyObject view )
        {
            Remove( view, RemoveReason.UserRequest );
        }

        /// <summary>
        /// Removes the specified view.
        /// </summary>
        /// <param name="view">The view.</param>
        /// <param name="reason">The reason.</param>
        protected void Remove( DependencyObject view, RemoveReason reason )
        {
            var args = new CancelEventArgs()
            {
                Cancel = false
            };

            RegionService.Conventions
                .GetViewDataContext( view, RegionService.Conventions.DefaultViewDataContextSearchBehavior )
                .As<IExpectViewClosingCallback>( i => i.OnViewClosing( args ) );

            if ( !args.Cancel )
            {
                OnRemove( view, reason );
                views.Remove( view );
                OnRemoved( view, reason );
            }
        }

        /// <summary>
        /// Called before the remove operation.
        /// </summary>
        /// <param name="view">The view.</param>
        /// <param name="reason">The reason.</param>
        protected abstract void OnRemove( DependencyObject view, RemoveReason reason );

        /// <summary>
        /// Called after the remove operation.
        /// </summary>
        /// <param name="view">The view.</param>
        /// <param name="reason">The reason.</param>
        protected virtual void OnRemoved( DependencyObject view, RemoveReason reason )
        {
            NotifyClosedAndEnsureRelease( view );
        }

        /// <summary>
        /// Gets the elements, in this region, of the given type T.
        /// </summary>
        /// <typeparam name="TView">The type of the elements to find.</typeparam>
        /// <returns>
        /// A list of elements of the given type T.
        /// </returns>
        public IEnumerable<TView> GetElements<TView>() where TView : DependencyObject
        {
            return views.OfType<TView>();
        }
    }

    /// <summary>
    /// The reason why an element is removed from an elements region.
    /// </summary>
    public enum RemoveReason
    {
        /// <summary>
        /// The request comes from the interactive user.
        /// </summary>
        UserRequest,

        /// <summary>
        /// The request is due to internal framework behavior.
        /// </summary>
        InternalRequest
    }
}