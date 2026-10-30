using Radical.Conversions;
using System;
using System.Windows.Controls;

namespace Radical.Windows.Behaviors
{
    /// <summary>
    /// A behavior that scrolls the selected item of a <see cref="ListView"/> into view when the selection changes.
    /// </summary>
    public class ScrollIntoViewForListView : RadicalBehavior<ListView>
    {
        /// <inheritdoc/>
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
        /// <inheritdoc/>
        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.SelectionChanged -= OnAssociatedObjectSelectionChanged;

        }
    }
}
