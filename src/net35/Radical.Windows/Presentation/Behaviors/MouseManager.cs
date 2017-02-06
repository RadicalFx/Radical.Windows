using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace Topics.Radical.Windows.Behaviors
{
    public class MouseManager
    {

        static MouseButtonEventHandler onMouseLeftButtonDown;
        static MouseButtonEventHandler onMouseRightButtonDown;
        static MouseButtonEventHandler onMouseDoubleClick;
        static ContextMenuEventHandler onContextMenuOpening;

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
                             typeof(Boolean),
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
            var ctrl = sender as Control;
            if (ctrl != null)
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
                var fe = owner as FrameworkElement;
                if (fe != null)
                    fe.Loaded += OnLoaded;
                var fce = owner as FrameworkContentElement;
                if (fce != null)
                    fce.Loaded += OnLoaded;
                SetOnLoadedAttached(owner, true);
            }

        }

        #endregion

        #region Attached Property: OnMouseLeftButtonDownProperty


        public static readonly DependencyProperty OnMouseLeftButtonDownProperty = DependencyProperty.RegisterAttached(
                             "OnMouseLeftButtonDown",
                             typeof(Point),
                             typeof(MouseManager),
                             new FrameworkPropertyMetadata(new Point(-1, -1), OnChanged)
                             {
                                 BindsTwoWayByDefault = true,
                                 DefaultUpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
                             });

        public static Point GetOnMouseLeftButtonDown(DependencyObject owner)
        {
            return (Point)owner.GetValue(OnMouseLeftButtonDownProperty);
        }

        public static void SetOnMouseLeftButtonDown(DependencyObject owner, Point point)
        {
            owner.SetValue(OnMouseLeftButtonDownProperty, point);
        }

        #endregion

        #region Attached Property: OnMouseRightButtonDownProperty


        public static readonly DependencyProperty OnMouseRightButtonDownProperty = DependencyProperty.RegisterAttached(
                             "OnMouseRightButtonDown",
                             typeof(Point),
                             typeof(MouseManager),
                             new FrameworkPropertyMetadata(new Point(-1, -1), OnChanged)
                             {
                                 BindsTwoWayByDefault = true,
                                 DefaultUpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
                             });

        public static Point GetOnMouseRightButtonDown(DependencyObject owner)
        {
            return (Point)owner.GetValue(OnMouseRightButtonDownProperty);
        }

        public static void SetOnMouseRightButtonDown(DependencyObject owner, Point point)
        {
            owner.SetValue(OnMouseRightButtonDownProperty, point);
        }

        #endregion

        #region Attached Property: OnMouseDoubleClickProperty


        public static readonly DependencyProperty OnMouseDoubleClickProperty = DependencyProperty.RegisterAttached(
                             "OnMouseDoubleClick",
                             typeof(Point),
                             typeof(MouseManager),
                             new FrameworkPropertyMetadata(new Point(-1, -1), OnChanged)
                             {
                                 BindsTwoWayByDefault = true,
                                 DefaultUpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
                             });

        public static Point GetOnMouseDoubleClick(DependencyObject owner)
        {
            return (Point)owner.GetValue(OnMouseDoubleClickProperty);
        }

        public static void SetOnMouseDoubleClick(DependencyObject owner, Point point)
        {
            owner.SetValue(OnMouseDoubleClickProperty, point);
        }

        #endregion

        #region Attached Property: OnContextMenuOpeningProperty

        public static readonly DependencyProperty OnContextMenuOpeningProperty = DependencyProperty.RegisterAttached(
                             "OnContextMenuOpening",
                             typeof(Point),
                             typeof(MouseManager),
                             new FrameworkPropertyMetadata(new Point(-1, -1), OnChanged)
                             {
                                 BindsTwoWayByDefault = true,
                                 DefaultUpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
                             });

        public static Point GetOnContextMenuOpening(DependencyObject owner)
        {
            return (Point)owner.GetValue(OnContextMenuOpeningProperty);
        }

        public static void SetOnContextMenuOpening(DependencyObject owner, Point point)
        {
            owner.SetValue(OnContextMenuOpeningProperty, point);
        }

        #endregion

    }
}
