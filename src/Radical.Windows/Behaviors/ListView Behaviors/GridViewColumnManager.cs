using System.Windows;
using System.Windows.Controls;

namespace Radical.Windows.Behaviors
{
    /// <summary>
    /// Exposes attached properties to configure <see cref="GridViewColumn"/> instances
    /// used by the <see cref="ListViewManager"/>.
    /// </summary>
    public static class GridViewColumnManager
    {
        #region Attached Property: Fill

        /// <summary>
        /// Identifies the Fill attached property: when <c>true</c> the column fills the available space
        /// of a list view whose AutoSizeColumns is enabled.
        /// </summary>
        public static readonly DependencyProperty FillProperty = DependencyProperty.RegisterAttached(
                                      "Fill",
                                      typeof(bool),
                                      typeof(GridViewColumnManager),
                                      new FrameworkPropertyMetadata(false));

        /// <summary>
        /// Gets a value indicating whether the given column fills the available space.
        /// </summary>
        /// <param name="owner">The column.</param>
        /// <returns><c>true</c> if the column fills the available space; otherwise, <c>false</c>.</returns>
        public static bool GetFill(GridViewColumn owner)
        {
            return (bool)owner.GetValue(FillProperty);
        }

        /// <summary>
        /// Sets a value indicating whether the given column fills the available space.
        /// </summary>
        /// <param name="owner">The column.</param>
        /// <param name="value"><c>true</c> to fill the available space; otherwise, <c>false</c>.</param>
        public static void SetFill(GridViewColumn owner, bool value)
        {
            owner.SetValue(FillProperty, value);
        }

        #endregion

        #region Attached Property: SortProperty

        /// <summary>
        /// Identifies the SortProperty attached property: the name of the property passed to the sort
        /// command when the column header is clicked.
        /// </summary>
        public static readonly DependencyProperty SortPropertyProperty = DependencyProperty.RegisterAttached(
                                      "SortProperty",
                                      typeof(string),
                                      typeof(GridViewColumnManager),
                                      new FrameworkPropertyMetadata(null));

        /// <summary>
        /// Gets the sort property of the given column.
        /// </summary>
        /// <param name="owner">The column.</param>
        /// <returns>The sort property name.</returns>
        public static string GetSortProperty(GridViewColumn owner)
        {
            return (string)owner.GetValue(SortPropertyProperty);
        }

        /// <summary>
        /// Sets the sort property of the given column.
        /// </summary>
        /// <param name="owner">The column.</param>
        /// <param name="value">The sort property name.</param>
        public static void SetSortProperty(GridViewColumn owner, string value)
        {
            owner.SetValue(SortPropertyProperty, value);
        }

        #endregion
    }
}
