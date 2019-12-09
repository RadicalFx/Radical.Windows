using System;

namespace Radical.Windows.Behaviors
{
    public class TextBoxCommandArgs : EventArgs
    {
        public TextBoxCommandArgs(System.Windows.Input.Key key, System.Windows.Input.ModifierKeys modifiers, object commandParameter)
        {
            Key = key;
            Modifiers = modifiers;
            CommandParameter = commandParameter;
        }

        public System.Windows.Input.Key Key { get; private set; }
        public System.Windows.Input.ModifierKeys Modifiers { get; private set; }

        public object CommandParameter { get; private set; }
    }
}
