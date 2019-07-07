using Radical.Conversions;
using Radical.Windows.Behaviors;
using System;
using System.Windows.Controls;

namespace Radical.Windows.Behaviors
{
    public class ScrollIntoViewForDataGrid : RadicalBehavior<DataGrid>
    {
        protected override void OnAttached()
        {
            base.OnAttached();
            this.AssociatedObject.SelectionChanged += OnAssociatedObjectSelectionChanged;
        }

        void OnAssociatedObjectSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            sender.As<DataGrid>(dataGrid =>
            {
                if (dataGrid.SelectedItem != null)
                {
                    dataGrid.Dispatcher.BeginInvoke(
                        (Action)(() =>
                        {
                            dataGrid.UpdateLayout();
                            if (dataGrid.SelectedItem != null)
                                dataGrid.ScrollIntoView(dataGrid.SelectedItem);
                        }));
                }
            });
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            this.AssociatedObject.SelectionChanged -= OnAssociatedObjectSelectionChanged;

        }
    }
}
