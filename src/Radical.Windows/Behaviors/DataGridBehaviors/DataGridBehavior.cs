using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Radical.Windows.Behaviors
{
    /// <summary>
    /// Exposes attached properties to customize the <see cref="DataGrid"/> layout.
    /// </summary>
    public class DataGridBehavior
    {
        /// <summary>
        /// Identifies the LastColumnFill attached property: when <c>true</c>, once the data grid is loaded,
        /// the last column fills the available space and the other columns are sized to fit their content.
        /// </summary>
        public static readonly DependencyProperty LastColumnFillProperty = DependencyProperty.RegisterAttached("LastColumnFill", typeof(bool), typeof(DataGridBehavior), new PropertyMetadata(default(bool), OnLastColumnFillChanged));

        /// <summary>
        /// Sets a value indicating whether the last column of the given data grid fills the available space.
        /// </summary>
        /// <param name="element">The data grid.</param>
        /// <param name="value"><c>true</c> to fill the available space with the last column; otherwise, <c>false</c>.</param>
        public static void SetLastColumnFill(DataGrid element, bool value)
        {
            element.SetValue(LastColumnFillProperty, value);
        }

        /// <summary>
        /// Gets a value indicating whether the last column of the given data grid fills the available space.
        /// </summary>
        /// <param name="element">The data grid.</param>
        /// <returns><c>true</c> if the last column fills the available space; otherwise, <c>false</c>.</returns>
        public static bool GetLastColumnFill(DataGrid element)
        {
            return (bool)element.GetValue(LastColumnFillProperty);
        }

        private static void OnLastColumnFillChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(d is DataGrid dataGrid)) return;

            dataGrid.Loaded -= OnDataGridLoaded;
            dataGrid.Loaded += OnDataGridLoaded;
        }

        private static void OnDataGridLoaded(object sender, RoutedEventArgs e)
        {
            if (!(sender is DataGrid dataGrid)) return;

            var lastColumn = dataGrid.Columns.LastOrDefault();
            if (lastColumn != null)
                lastColumn.Width = new DataGridLength(1, DataGridLengthUnitType.Star);

            // Auto-fit all other columns
            foreach (var column in dataGrid.Columns)
            {
                if (column == lastColumn) break;

                double beforeWidth = column.ActualWidth;
                column.Width = new DataGridLength(1, DataGridLengthUnitType.SizeToCells);
                double sizeCellsWidth = column.ActualWidth;
                column.Width = new DataGridLength(1, DataGridLengthUnitType.SizeToHeader);
                double sizeHeaderWidth = column.ActualWidth;
                column.MinWidth = Math.Max(beforeWidth, Math.Max(sizeCellsWidth, sizeHeaderWidth));
            }
        }
    }
}