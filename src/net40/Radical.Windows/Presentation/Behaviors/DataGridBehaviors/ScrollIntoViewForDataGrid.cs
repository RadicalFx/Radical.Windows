using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using Topics.Radical.Windows.Behaviors;

namespace Topics.Radical.Windows.Presentation.Behaviors
{
    class ScrollIntoViewForDataGrid : RadicalBehavior<DataGrid>
    {
        protected override void OnAttached()
        {
            base.OnAttached();
            this.AssociatedObject.SelectionChanged += AssociatedObject_SelectionChanged;
        }

        void AssociatedObject_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is DataGrid)
            {
                DataGrid dataGrid = (sender as DataGrid);
                if (dataGrid.SelectedItem != null)
                {
                    dataGrid.Dispatcher.BeginInvoke(
                        (Action)(() =>
                        {
                            dataGrid.UpdateLayout();
                            if (dataGrid.SelectedItem !=
                                null)
                                dataGrid.ScrollIntoView(
                                    dataGrid.SelectedItem);
                        }));
                }
            }
        }
        protected override void OnDetaching()
        {
            base.OnDetaching();
            this.AssociatedObject.SelectionChanged -=
                AssociatedObject_SelectionChanged;

        }
    }
}
