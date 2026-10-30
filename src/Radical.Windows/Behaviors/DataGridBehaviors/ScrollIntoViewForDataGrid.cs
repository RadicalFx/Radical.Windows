using Radical.Conversions;
using System;
using System.Windows.Controls;

namespace Radical.Windows.Behaviors
{
    /// <summary>
    /// A behavior that scrolls the selected item of a <see cref="DataGrid"/> into view when the selection changes.
    /// </summary>
    public class ScrollIntoViewForDataGrid : RadicalBehavior<DataGrid>
    {
        /// <inheritdoc/>
        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.SelectionChanged += OnAssociatedObjectSelectionChanged;
        }

        void OnAssociatedObjectSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is DataGrid { SelectedItem: not null } dataGrid)
            {
                dataGrid.Dispatcher.BeginInvoke(
                    (Action)(() =>
                    {
                        dataGrid.UpdateLayout();
                        if (dataGrid.SelectedItem != null)
                            dataGrid.ScrollIntoView(dataGrid.SelectedItem);
                    }));
            }
        }

        /// <inheritdoc/>
        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.SelectionChanged -= OnAssociatedObjectSelectionChanged;

        }
    }
}
