using Radical.Conversions;
using System;
using System.Windows.Controls;

namespace Radical.Windows.Behaviors
{
    public class ScrollIntoViewForListView : RadicalBehavior<ListView>
    {
        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.SelectionChanged += OnAssociatedObjectSelectionChanged;
        }

        void OnAssociatedObjectSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is ListView { SelectedItem: not null } listView)
            {
                listView.Dispatcher.BeginInvoke(
                    (Action)(() =>
                    {
                        listView.UpdateLayout();
                        if (listView.SelectedItem != null)
                            listView.ScrollIntoView(listView.SelectedItem);
                    }));
            }
        }
        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.SelectionChanged -= OnAssociatedObjectSelectionChanged;

        }
    }
}
