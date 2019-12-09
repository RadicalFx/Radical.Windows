using Microsoft.Xaml.Behaviors;
using System.Windows.Controls;
using System.Windows.Input;

namespace Radical.Windows.Behaviors
{
    public sealed class DisableUndoManagerBehavior : Behavior<TextBox>
    {
        protected override void OnAttached()
        {
            base.OnAttached();

            var cb = new CommandBinding();
            cb.Command = ApplicationCommands.Undo;
            cb.CanExecute += (s, e) => e.CanExecute = AssociatedObject.IsFocused;
            cb.Executed += (s, e) => e.Handled = true;

            AssociatedObject.CommandBindings.Add(cb);
        }
    }
}
