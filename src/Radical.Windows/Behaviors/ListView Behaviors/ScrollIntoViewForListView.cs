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
            this.AssociatedObject.SelectionChanged += OnAssociatedObjectSelectionChanged;
        }

        void OnAssociatedObjectSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            sender.As<ListView>(listView =>
            {
                if (listView.SelectedItem != null)
                {
                    listView.Dispatcher.BeginInvoke(
                        (Action)(() =>
                        {
                            listView.UpdateLayout();
                            if (listView.SelectedItem != null)
                                listView.ScrollIntoView(listView.SelectedItem);
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
