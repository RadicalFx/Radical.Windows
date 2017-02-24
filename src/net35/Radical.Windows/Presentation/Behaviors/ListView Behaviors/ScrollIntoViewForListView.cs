using System;
using System.Windows.Controls;

namespace Topics.Radical.Windows.Behaviors
{
    public class ScrollIntoViewForListView : RadicalBehavior<ListView>
    {
        protected override void OnAttached()
        {
            base.OnAttached();
            this.AssociatedObject.SelectionChanged += AssociatedObject_SelectionChanged;
        }

        void AssociatedObject_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is ListView)
            {
                ListView listView = (sender as ListView);
                if (listView.SelectedItem != null)
                {
                    listView.Dispatcher.BeginInvoke(
                        (Action)(() =>
                        {
                            listView.UpdateLayout();
                            if (listView.SelectedItem !=
                                null)
                                listView.ScrollIntoView(
                                    listView.SelectedItem);
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
