using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using Topics.Radical.Conversions;
using Topics.Radical.Windows.Behaviors;

namespace Topics.Radical.Windows.Presentation.Behaviors
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
