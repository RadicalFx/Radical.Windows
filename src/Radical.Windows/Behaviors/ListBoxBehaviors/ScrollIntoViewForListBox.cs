using System;
using System.Windows.Controls;

namespace Radical.Windows.Behaviors
{
    /// <summary>
    /// A behavior that scrolls the selected item of a <see cref="ListBox"/> into view when the selection changes.
    /// </summary>
    public class ScrollIntoViewForListBox : RadicalBehavior<ListBox>
    {
        /// <inheritdoc/>
        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.SelectionChanged += AssociatedObject_SelectionChanged;
        }

        void AssociatedObject_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is ListBox)
            {
                ListBox listBox = (sender as ListBox);
                if (listBox.SelectedItem != null)
                {
                    listBox.Dispatcher.BeginInvoke(
                        (Action)(() =>
                        {
                            listBox.UpdateLayout();
                            if (listBox.SelectedItem !=
                                null)
                                listBox.ScrollIntoView(
                                    listBox.SelectedItem);
                        }));
                }
            }
        }
        /// <inheritdoc/>
        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.SelectionChanged -=
                AssociatedObject_SelectionChanged;

        }
    }
}
