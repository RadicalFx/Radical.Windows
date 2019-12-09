using System.Windows;
using System.Windows.Controls;

namespace Radical.Windows.Behaviors
{
    public static class GridViewColumnManager
    {
        #region Attached Property: Fill

        public static readonly DependencyProperty FillProperty = DependencyProperty.RegisterAttached(
                                      "Fill",
                                      typeof(bool),
                                      typeof(GridViewColumnManager),
                                      new FrameworkPropertyMetadata(false));

        public static bool GetFill(GridViewColumn owner)
        {
            return (bool)owner.GetValue(FillProperty);
        }

        public static void SetFill(GridViewColumn owner, bool value)
        {
            owner.SetValue(FillProperty, value);
        }

        #endregion

        #region Attached Property: SortProperty

        public static readonly DependencyProperty SortPropertyProperty = DependencyProperty.RegisterAttached(
                                      "SortProperty",
                                      typeof(string),
                                      typeof(GridViewColumnManager),
                                      new FrameworkPropertyMetadata(null));

        public static string GetSortProperty(GridViewColumn owner)
        {
            return (string)owner.GetValue(SortPropertyProperty);
        }

        public static void SetSortProperty(GridViewColumn owner, string value)
        {
            owner.SetValue(SortPropertyProperty, value);
        }

        #endregion
    }
}
