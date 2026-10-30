using System.Diagnostics;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;

namespace Radical.Windows.Behaviors
{
    /// <summary>
    /// A behavior that shows content in an adorner overlaying the associated element.
    /// </summary>
    public class OverlayBehavior : RadicalBehavior<FrameworkElement>
    {
        #region Dependency Property: Content

        /// <summary>
        /// Content Dependency property
        /// </summary>
        public static readonly DependencyProperty ContentProperty = DependencyProperty.Register(
            "Content",
            typeof(object),
            typeof(OverlayBehavior),
            new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the content.
        /// </summary>
        /// <value>The content.</value>
        public object Content
        {
            get { return (object)GetValue(ContentProperty); }
            set { SetValue(ContentProperty, value); }
        }

        #endregion

        #region Dependency Property: IsVisible

        /// <summary>
        /// Identifies the <see cref="IsVisible"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsVisibleProperty = DependencyProperty.Register(
            "IsVisible",
            typeof(bool),
            typeof(OverlayBehavior),
            new PropertyMetadata(true, (s, e) =>
           {
               ((OverlayBehavior)s).OnIsVisibleChanged(e);
           }));

        /// <summary>
        /// Gets or sets a value indicating whether the overlay is visible. Defaults to <c>true</c>.
        /// </summary>
        public bool IsVisible
        {
            get { return (bool)GetValue(IsVisibleProperty); }
            set { SetValue(IsVisibleProperty, value); }
        }

        #endregion

        #region Dependency Property: DisableAdornedElement

        /// <summary>
        /// Identifies the <see cref="DisableAdornedElement"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty DisableAdornedElementProperty = DependencyProperty.Register(
            "DisableAdornedElement",
            typeof(bool),
            typeof(OverlayBehavior),
            new PropertyMetadata(false));

        /// <summary>
        /// Gets or sets a value indicating whether the associated element is disabled while the overlay is visible.
        /// </summary>
        public bool DisableAdornedElement
        {
            get { return (bool)GetValue(DisableAdornedElementProperty); }
            set { SetValue(DisableAdornedElementProperty, value); }
        }

        #endregion

        #region Dependency Property: IsHitTestVisible

        /// <summary>
        /// Identifies the <see cref="IsHitTestVisible"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsHitTestVisibleProperty = DependencyProperty.Register(
            "IsHitTestVisible",
            typeof(bool),
            typeof(OverlayBehavior),
            new PropertyMetadata(true,(s, e) =>
            {
                ((OverlayBehavior)s).IsHitTestVisibleChanged(e);
            }));

        /// <summary>
        /// Gets or sets a value indicating whether the overlay is hit test visible. Defaults to <c>true</c>.
        /// </summary>
        public bool IsHitTestVisible
        {
            get { return (bool)GetValue(IsHitTestVisibleProperty); }
            set { SetValue(IsHitTestVisibleProperty, value); }
        }

        #endregion

        #region Dependency Property: Background

        /// <summary>
        /// Identifies the <see cref="Background"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty BackgroundProperty = DependencyProperty.Register(
            "Background",
            typeof(Brush),
            typeof(OverlayBehavior),
            new PropertyMetadata(null, (s, e) =>
           {
               ((OverlayBehavior)s).OnBackgroundChanged(e);
           }));

        /// <summary>
        /// Gets or sets the brush used to paint the overlay background.
        /// </summary>
        public Brush Background
        {
            get { return (Brush)GetValue(BackgroundProperty); }
            set { SetValue(BackgroundProperty, value); }
        }

        #endregion

        //#region Dependency Property: BackgroundOpacity

        //public static readonly DependencyProperty BackgroundOpacityProperty = DependencyProperty.Register(
        //    "BackgroundOpacity",
        //    typeof( Double ),
        //    typeof( OverlayBehavior ),
        //    new PropertyMetadata( 1d, ( s, e ) =>
        //    {
        //        ( ( OverlayBehavior )s ).OnBackgroundOpacityChanged( e );
        //    } ) );

        //public Double BackgroundOpacity
        //{
        //    get { return ( Double )this.GetValue( BackgroundOpacityProperty ); }
        //    set { this.SetValue( BackgroundOpacityProperty, value ); }
        //}

        //#endregion

        private void OnBackgroundChanged(DependencyPropertyChangedEventArgs e)
        {
            if (isAdornerVisible)
            {
                adorner.InvalidateVisual();
            }
        }

        private void IsHitTestVisibleChanged(DependencyPropertyChangedEventArgs e)
        {
            if (adorner != null)
            {
               adorner.IsHitTestVisible = IsHitTestVisible;
            }
            if (isAdornerVisible)
            {
                adorner.InvalidateVisual();
            }
        }

        private void OnIsVisibleChanged(DependencyPropertyChangedEventArgs e)
        {
            Toggle();
        }

        //private void OnBackgroundOpacityChanged( DependencyPropertyChangedEventArgs e )
        //{
        //    if ( this.isAdornerVisible )
        //    {
        //        this.adorner.InvalidateVisual();
        //    }
        //}

        ContentOverlayAdorner adorner;
        bool isAdornerVisible;
        private bool wasEnabled;

        /// <inheritdoc/>
        protected override void OnAttached()
        {
            base.OnAttached();

            if (Content != null)
            {
                AssociatedObject.Loaded += new RoutedEventHandler(OnLoaded);
            }
        }

        void OnLoaded(object sender, RoutedEventArgs e)
        {
            if (IsVisible)
            {
                ShowAdorner();
            }
        }

        private void Toggle()
        {
            if (IsVisible)
            {
                ShowAdorner();
            }
            else
            {
                HideAdorner();
            }
        }

        void ShowAdorner()
        {
            if (!isAdornerVisible)
            {
                var layer = AdornerLayer.GetAdornerLayer(AssociatedObject);
                Debug.WriteLineIf(layer == null, "Overlay: cannot find any AdornerLayer on the given element.");

                if (layer != null)
                {
                    adorner = new ContentOverlayAdorner(AssociatedObject, Content)
                    {
                        IsHitTestVisible = IsHitTestVisible,
                        Background = Background
                    };
                    layer.Add(adorner);

                    wasEnabled = AssociatedObject.IsEnabled;
                    AssociatedObject.IsEnabled = !DisableAdornedElement;

                    isAdornerVisible = true;
                }
            }
        }

        void HideAdorner()
        {
            if (isAdornerVisible && adorner != null)
            {
                var layer = AdornerLayer.GetAdornerLayer(AssociatedObject);
                Debug.WriteLineIf(layer == null, "Overlay: cannot find any AdornerLayer on the given element.");

                if (layer != null)
                {
                    layer.Remove(adorner);
                    adorner = null;
                    isAdornerVisible = false;
                }

                AssociatedObject.IsEnabled = wasEnabled;
            }
        }

        /// <inheritdoc/>
        protected override void OnDetaching()
        {
            AssociatedObject.Loaded -= new RoutedEventHandler(OnLoaded);
            HideAdorner();

            base.OnDetaching();
        }
    }
}
