using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace Radical.Windows.Behaviors
{
    /// <summary>
    /// Exposes attached properties that capture, as two-way bindable points, the position
    /// of the mouse when buttons are pressed on an element.
    /// </summary>
    public class MouseManager
    {

        static readonly MouseButtonEventHandler onMouseLeftButtonDown;
        static readonly MouseButtonEventHandler onMouseRightButtonDown;
        static readonly MouseButtonEventHandler onMouseDoubleClick;
        static readonly ContextMenuEventHandler onContextMenuOpening;

        static MouseManager()
        {
            onMouseLeftButtonDown = (s, e) =>
            {
                var p = Mouse.GetPosition((IInputElement)s);
                SetOnMouseLeftButtonDown((DependencyObject)s, p);
            };

            onMouseRightButtonDown = (s, e) =>
            {
                var p = Mouse.GetPosition((IInputElement)s);
                SetOnMouseRightButtonDown((DependencyObject)s, p);
            };

            onMouseDoubleClick = (s, e) =>
            {
                var p = Mouse.GetPosition((IInputElement)s);
                SetOnMouseDoubleClick((DependencyObject)s, p);
            };

            onContextMenuOpening = (s, e) =>
            {
                var p = Mouse.GetPosition((IInputElement)s);
                SetOnContextMenuOpening((DependencyObject)s, p);
            };

        }

        #region Attached Property: OnLoadedAttachedProperty

        static readonly DependencyProperty OnLoadedAttachedProperty = DependencyProperty.RegisterAttached(
                             "OnLoadedAttached",
                             typeof(bool),
                             typeof(MouseManager),
                             new FrameworkPropertyMetadata(false));

        static bool GetOnLoadedAttached(DependencyObject owner)
        {
            return (bool)owner.GetValue(OnLoadedAttachedProperty);
        }

        static void SetOnLoadedAttached(DependencyObject owner, bool value)
        {
            owner.SetValue(OnLoadedAttachedProperty, value);
        }

        static void OnLoaded(object sender, RoutedEventArgs e)
        {
            if (sender is Control ctrl)
            {
                ctrl.MouseLeftButtonDown += onMouseLeftButtonDown;
                ctrl.MouseRightButtonDown += onMouseRightButtonDown;
                ctrl.MouseDoubleClick += onMouseDoubleClick;
                ctrl.ContextMenuOpening += onContextMenuOpening;
            }

        }
        private static void OnChanged(DependencyObject owner, DependencyPropertyChangedEventArgs e)
        {
            var isLoadedAttached = GetOnLoadedAttached(owner);

            if (!isLoadedAttached && !DesignTimeHelper.GetIsInDesignMode())
            {
                if (owner is FrameworkElement fe)
                    fe.Loaded += OnLoaded;
                if (owner is FrameworkContentElement fce)
                    fce.Loaded += OnLoaded;
                SetOnLoadedAttached(owner, true);
            }

        }

        #endregion

        #region Attached Property: OnMouseLeftButtonDownProperty


        /// <summary>
        /// Identifies the OnMouseLeftButtonDown attached property: the position, relative to the element,
        /// where the left mouse button was last pressed. Defaults to (-1, -1).
        /// </summary>
        public static readonly DependencyProperty OnMouseLeftButtonDownProperty = DependencyProperty.RegisterAttached(
                             "OnMouseLeftButtonDown",
                             typeof(Point),
                             typeof(MouseManager),
                             new FrameworkPropertyMetadata(new Point(-1, -1), OnChanged)
                             {
                                 BindsTwoWayByDefault = true,
                                 DefaultUpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
                             });

        /// <summary>
        /// Gets the position where the left mouse button was last pressed on the given element.
        /// </summary>
        /// <param name="owner">The element.</param>
        /// <returns>The position, relative to the element.</returns>
        public static Point GetOnMouseLeftButtonDown(DependencyObject owner)
        {
            return (Point)owner.GetValue(OnMouseLeftButtonDownProperty);
        }

        /// <summary>
        /// Sets the position where the left mouse button was last pressed on the given element.
        /// </summary>
        /// <param name="owner">The element.</param>
        /// <param name="point">The position, relative to the element.</param>
        public static void SetOnMouseLeftButtonDown(DependencyObject owner, Point point)
        {
            owner.SetValue(OnMouseLeftButtonDownProperty, point);
        }

        #endregion

        #region Attached Property: OnMouseRightButtonDownProperty


        /// <summary>
        /// Identifies the OnMouseRightButtonDown attached property: the position, relative to the element,
        /// where the right mouse button was last pressed. Defaults to (-1, -1).
        /// </summary>
        public static readonly DependencyProperty OnMouseRightButtonDownProperty = DependencyProperty.RegisterAttached(
                             "OnMouseRightButtonDown",
                             typeof(Point),
                             typeof(MouseManager),
                             new FrameworkPropertyMetadata(new Point(-1, -1), OnChanged)
                             {
                                 BindsTwoWayByDefault = true,
                                 DefaultUpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
                             });

        /// <summary>
        /// Gets the position where the right mouse button was last pressed on the given element.
        /// </summary>
        /// <param name="owner">The element.</param>
        /// <returns>The position, relative to the element.</returns>
        public static Point GetOnMouseRightButtonDown(DependencyObject owner)
        {
            return (Point)owner.GetValue(OnMouseRightButtonDownProperty);
        }

        /// <summary>
        /// Sets the position where the right mouse button was last pressed on the given element.
        /// </summary>
        /// <param name="owner">The element.</param>
        /// <param name="point">The position, relative to the element.</param>
        public static void SetOnMouseRightButtonDown(DependencyObject owner, Point point)
        {
            owner.SetValue(OnMouseRightButtonDownProperty, point);
        }

        #endregion

        #region Attached Property: OnMouseDoubleClickProperty


        /// <summary>
        /// Identifies the OnMouseDoubleClick attached property: the position, relative to the element,
        /// of the last double click. Defaults to (-1, -1).
        /// </summary>
        public static readonly DependencyProperty OnMouseDoubleClickProperty = DependencyProperty.RegisterAttached(
                             "OnMouseDoubleClick",
                             typeof(Point),
                             typeof(MouseManager),
                             new FrameworkPropertyMetadata(new Point(-1, -1), OnChanged)
                             {
                                 BindsTwoWayByDefault = true,
                                 DefaultUpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
                             });

        /// <summary>
        /// Gets the position of the last double click on the given element.
        /// </summary>
        /// <param name="owner">The element.</param>
        /// <returns>The position, relative to the element.</returns>
        public static Point GetOnMouseDoubleClick(DependencyObject owner)
        {
            return (Point)owner.GetValue(OnMouseDoubleClickProperty);
        }

        /// <summary>
        /// Sets the position of the last double click on the given element.
        /// </summary>
        /// <param name="owner">The element.</param>
        /// <param name="point">The position, relative to the element.</param>
        public static void SetOnMouseDoubleClick(DependencyObject owner, Point point)
        {
            owner.SetValue(OnMouseDoubleClickProperty, point);
        }

        #endregion

        #region Attached Property: OnContextMenuOpeningProperty

        /// <summary>
        /// Identifies the OnContextMenuOpening attached property: the position, relative to the element,
        /// where the context menu was last opened. Defaults to (-1, -1).
        /// </summary>
        public static readonly DependencyProperty OnContextMenuOpeningProperty = DependencyProperty.RegisterAttached(
                             "OnContextMenuOpening",
                             typeof(Point),
                             typeof(MouseManager),
                             new FrameworkPropertyMetadata(new Point(-1, -1), OnChanged)
                             {
                                 BindsTwoWayByDefault = true,
                                 DefaultUpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
                             });

        /// <summary>
        /// Gets the position where the context menu was last opened on the given element.
        /// </summary>
        /// <param name="owner">The element.</param>
        /// <returns>The position, relative to the element.</returns>
        public static Point GetOnContextMenuOpening(DependencyObject owner)
        {
            return (Point)owner.GetValue(OnContextMenuOpeningProperty);
        }

        /// <summary>
        /// Sets the position where the context menu was last opened on the given element.
        /// </summary>
        /// <param name="owner">The element.</param>
        /// <param name="point">The position, relative to the element.</param>
        public static void SetOnContextMenuOpening(DependencyObject owner, Point point)
        {
            owner.SetValue(OnContextMenuOpeningProperty, point);
        }

        #endregion

    }
}
