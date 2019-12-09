using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Radical.Windows.Presentation.Regions
{
    /// <summary>
    /// A switching elements region hosted by a TabControl.
    /// </summary>
    public class TabControlRegion : SwitchingElementsRegion<TabControl>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TabControlRegion"/> class.
        /// </summary>
        public TabControlRegion()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TabControlRegion"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        public TabControlRegion( string name )
        {
            Name = name;
        }

        /// <summary>
        /// Called before the add operation.
        /// </summary>
        /// <param name="view">The view.</param>
        protected override void OnAdd( DependencyObject view )
        {
            var tabItem = new TabItem();
            tabItem.Content = view;

            var header = TryGetHeader( view );
            if( header != null )
            {
                tabItem.Header = header;
            }

            Element.Items.Add( tabItem );
        }

        /// <summary>
        /// Tries the get header.
        /// </summary>
        /// <param name="view">The view.</param>
        /// <returns></returns>
        protected virtual object TryGetHeader( DependencyObject view )
        {
            return RegionHeaderedElement.GetHeader( view );
        }

        /// <summary>
        /// Called after the add operation.
        /// </summary>
        /// <param name="view">The view.</param>
        /// <param name="reason">The reason.</param>
        protected override void OnRemove( DependencyObject view, RemoveReason reason )
        {
            var element = GetElement( view );
            if( element != null )
            {
                Element.Items.Remove( element );
            }
        }

        /// <summary>
        /// Activates the specified content.
        /// </summary>
        /// <param name="content">The content.</param>
        public override void Activate( DependencyObject content )
        {
            var element = GetElement( content );
            if( element != null )
            {
                Element.SelectedIndex = Element.Items.IndexOf( element );
                OnActiveContentChanged();
            }
        }

        /// <summary>
        /// Gets the content of the active.
        /// </summary>
        /// <value>
        /// The content of the active.
        /// </value>
        public override DependencyObject ActiveContent
        {
            get
            {
                if( Element.SelectedIndex == -1 ) return null;
                var selectedItem = Element.Items[ Element.SelectedIndex ];
                var tabItem = selectedItem as TabItem;
                if( tabItem != null )
                {
                    return tabItem.Content as DependencyObject;
                }
                return selectedItem as DependencyObject;
            }
        }

        private object GetElement( DependencyObject content )
        {
            if( Element.Items.Contains( content ) )
            {
                return content;
            }

            var element = Element.Items
                              .OfType<TabItem>()
                              .Where( t => t.Content == content )
                              .FirstOrDefault( t => Element.Items.Contains( t ) );
            return element;
        }
    }
}
