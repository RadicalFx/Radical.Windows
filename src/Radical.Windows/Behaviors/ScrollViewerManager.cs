using System.Windows;
using System.Windows.Controls;

namespace Radical.Windows.Behaviors
{
    /// <summary>
    /// Exposes attached properties to two-way bind the scroll offsets of a <see cref="ScrollViewer"/>.
    /// </summary>
    public class ScrollViewerManager
    {

        #region Attached Property: IsSetup

        static readonly DependencyProperty IsSetupProperty = DependencyProperty.RegisterAttached(
                            "IsSetup",
                            typeof(bool),
                            typeof(ScrollViewerManager),
                            new FrameworkPropertyMetadata(false));

        static bool GetIsSetup(DependencyObject owner)
        {
            return (bool)owner.GetValue(IsSetupProperty);
        }

        static void SetIsSetup(DependencyObject owner, bool value)
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

        /// <summary>
        /// Identifies the HorizontalOffset attached property. Defaults to -1, meaning not set.
        /// </summary>
        public static readonly DependencyProperty HorizontalOffsetProperty = DependencyProperty.RegisterAttached(
                            "HorizontalOffset",
                            typeof(double),
                            typeof(ScrollViewerManager),
                            new FrameworkPropertyMetadata((double)-1, OnHorizontalOffsetChanged)
                            {
                                BindsTwoWayByDefault = true
                            });

        /// <summary>
        /// Gets the horizontal offset of the given scroll viewer.
        /// </summary>
        /// <param name="owner">The scroll viewer.</param>
        /// <returns>The horizontal offset.</returns>
        public static double GetHorizontalOffset(DependencyObject owner)
        {
            return (double)owner.GetValue(HorizontalOffsetProperty);
        }

        /// <summary>
        /// Sets the horizontal offset of the given scroll viewer.
        /// </summary>
        /// <param name="owner">The scroll viewer.</param>
        /// <param name="value">The horizontal offset.</param>
        public static void SetHorizontalOffset(DependencyObject owner, double value)
        {
            owner.SetValue(HorizontalOffsetProperty, value);
        }

        private static void OnHorizontalOffsetChanged(DependencyObject owner, DependencyPropertyChangedEventArgs e)
        {
            if (owner is ScrollViewer viewer)
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

        /// <summary>
        /// Identifies the VerticalOffset attached property. Defaults to -1, meaning not set.
        /// </summary>
        public static readonly DependencyProperty VerticalOffsetProperty = DependencyProperty.RegisterAttached(
                            "VerticalOffset",
                            typeof(double),
                            typeof(ScrollViewerManager),
                            new FrameworkPropertyMetadata((double)-1, OnVerticalOffsetChanged)
                            {
                                BindsTwoWayByDefault = true
                            });

        /// <summary>
        /// Gets the vertical offset of the given scroll viewer.
        /// </summary>
        /// <param name="owner">The scroll viewer.</param>
        /// <returns>The vertical offset.</returns>
        public static double GetVerticalOffset(DependencyObject owner)
        {
            return (double)owner.GetValue(VerticalOffsetProperty);
        }
        /// <summary>
        /// Sets the vertical offset of the given scroll viewer.
        /// </summary>
        /// <param name="owner">The scroll viewer.</param>
        /// <param name="value">The vertical offset.</param>
        public static void SetVerticalOffset(DependencyObject owner, double value)
        {
            owner.SetValue(VerticalOffsetProperty, value);
        }

        private static void OnVerticalOffsetChanged(DependencyObject owner, DependencyPropertyChangedEventArgs e)
        {
            if (owner is ScrollViewer viewer)
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
