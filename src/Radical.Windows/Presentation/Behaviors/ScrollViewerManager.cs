using System;
using System.Windows;
using System.Windows.Controls;

namespace Radical.Windows.Presentation.Behaviors
{
    public class ScrollViewerManager
    {

        #region Attached Property: IsSetup

        static readonly DependencyProperty IsSetupProperty = DependencyProperty.RegisterAttached(
                            "IsSetup",
                            typeof(Boolean),
                            typeof(ScrollViewerManager),
                            new FrameworkPropertyMetadata(false));

        static bool GetIsSetup(DependencyObject owner)
        {
            return (bool)owner.GetValue(IsSetupProperty);
        }

        static void SetIsSetup(DependencyObject owner, Boolean value)
        {
            owner.SetValue(IsSetupProperty, value);
        }

        private static void OnSourceScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            SetHorizontalOffset((DependencyObject)sender, e.HorizontalOffset);
            SetVerticalOffset((DependencyObject)sender, e.VerticalOffset);
        }

        #endregion

        #region Attached Property: HorizontalOffsetProperty

        public static readonly DependencyProperty HorizontalOffsetProperty = DependencyProperty.RegisterAttached(
                            "HorizontalOffset",
                            typeof(double),
                            typeof(ScrollViewerManager),
                            new FrameworkPropertyMetadata((double)-1, OnHorizontalOffsetChanged)
                            {
                                BindsTwoWayByDefault = true
                            });

        public static double GetHorizontalOffset(DependencyObject owner)
        {
            return (double)owner.GetValue(HorizontalOffsetProperty);
        }

        public static void SetHorizontalOffset(DependencyObject owner, double value)
        {
            owner.SetValue(HorizontalOffsetProperty, value);
        }

        private static void OnHorizontalOffsetChanged(DependencyObject owner, DependencyPropertyChangedEventArgs e)
        {
            var viewer = owner as ScrollViewer;
            if (viewer != null)
            {
                if (!GetIsSetup(viewer))
                {
                    viewer.ScrollChanged += OnSourceScrollChanged;
                    SetIsSetup(viewer, true);
                }

                viewer.ScrollToHorizontalOffset((double)e.NewValue);
            }
        }

        #endregion

        #region Attached Property: VerticalOffsetProperty

        public static readonly DependencyProperty VerticalOffsetProperty = DependencyProperty.RegisterAttached(
                            "VerticalOffset",
                            typeof(double),
                            typeof(ScrollViewerManager),
                            new FrameworkPropertyMetadata((double)-1, OnVerticalOffsetChanged)
                            {
                                BindsTwoWayByDefault = true
                            });

        public static double GetVerticalOffset(DependencyObject owner)
        {
            return (double)owner.GetValue(VerticalOffsetProperty);
        }
        public static void SetVerticalOffset(DependencyObject owner, double value)
        {
            owner.SetValue(VerticalOffsetProperty, value);
        }

        private static void OnVerticalOffsetChanged(DependencyObject owner, DependencyPropertyChangedEventArgs e)
        {
            var viewer = owner as ScrollViewer;
            if (viewer != null)
            {
                if (!GetIsSetup(viewer))
                {
                    viewer.ScrollChanged += OnSourceScrollChanged;
                    SetIsSetup(viewer, true);
                }

                viewer.ScrollToVerticalOffset((double)e.NewValue);
            }
        }

        #endregion

    }
}
