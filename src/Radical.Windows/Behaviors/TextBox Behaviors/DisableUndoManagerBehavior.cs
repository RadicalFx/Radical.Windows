using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Xaml.Behaviors;

namespace Radical.Windows.Behaviors
{
    public sealed class DisableUndoManagerBehavior : Behavior<TextBox>
    {
        protected override void OnAttached()
        {
            base.OnAttached();

            var cb = new CommandBinding();
            cb.Command = ApplicationCommands.Undo;
            cb.CanExecute += (s, e) => e.CanExecute = this.AssociatedObject.IsFocused;
            cb.Executed += (s, e) => e.Handled = true;

            this.AssociatedObject.CommandBindings.Add(cb);
        }
    }
}
