using System;

namespace Radical.Windows.Behaviors
{
    /// <summary>
    /// The parameter passed to text box commands, describing the pressed key.
    /// </summary>
    public class TextBoxCommandArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TextBoxCommandArgs"/> class.
        /// </summary>
        /// <param name="key">The pressed key.</param>
        /// <param name="modifiers">The pressed modifier keys.</param>
        /// <param name="commandParameter">The command parameter configured on the text box.</param>
        public TextBoxCommandArgs(System.Windows.Input.Key key, System.Windows.Input.ModifierKeys modifiers, object commandParameter)
        {
            Key = key;
            Modifiers = modifiers;
            CommandParameter = commandParameter;
        }

        /// <summary>
        /// Gets the pressed key.
        /// </summary>
        public System.Windows.Input.Key Key { get; private set; }
        /// <summary>
        /// Gets the pressed modifier keys.
        /// </summary>
        public System.Windows.Input.ModifierKeys Modifiers { get; private set; }

        /// <summary>
        /// Gets the command parameter configured on the text box.
        /// </summary>
        public object CommandParameter { get; private set; }
    }
}
