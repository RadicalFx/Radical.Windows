using Radical.Linq;
using Radical.Windows.Input;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Radical.Windows.Behaviors
{
    /// <summary>
    /// A behavior that executes a command when keys are pressed in the associated <see cref="TextBox"/>.
    /// The command is executed if the pressed key matches one of the command gestures, or is Enter
    /// when the command has no gestures.
    /// </summary>
    public class TextBoxCommandBehavior : RadicalBehavior<TextBox>, ICommandSource
    {
        #region Dependency Property: Command

        /// <summary>
        /// Identifies the <see cref="Command"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CommandProperty = DependencyProperty.Register(
            "Command",
            typeof(ICommand),
            typeof(TextBoxCommandBehavior),
            new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the command to execute.
        /// </summary>
        public ICommand Command
        {
            get { return (ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }

        #endregion

        /// <summary>
        /// Gets or sets the parameter passed to the command, wrapped in a <see cref="TextBoxCommandArgs"/>.
        /// </summary>
        public object CommandParameter
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the command target: the associated text box.
        /// </summary>
        public IInputElement CommandTarget
        {
            get { return AssociatedObject; }
        }

        readonly KeyEventHandler onPreviewKeyDown;

        /// <summary>
        /// Initializes a new instance of the <see cref="TextBoxCommandBehavior"/> class.
        /// </summary>
        public TextBoxCommandBehavior()
        {
            onPreviewKeyDown = (s, e) =>
            {
                var d = (DependencyObject)s;

                var cmd = Command;
                var prm = CommandParameter;

                if (cmd.CanExecute(prm))
                {
                    var gestures = cmd.GetGestures();
                    var senderGestures = gestures.Where(gesture => gesture.Matches(d, e));

                    if (((gestures.None() && e.Key == Key.Enter) || senderGestures.Any()))
                    {
                        var k = e.Key;
                        var m = ModifierKeys.None;

                        if (senderGestures.Any())
                        {
                            var gesture = senderGestures.First();
                            if (gesture is KeyGesture keygesture)
                            {
                                k = keygesture.Key;
                                m = keygesture.Modifiers;
                            }
                        }

                        var args = new TextBoxCommandArgs(k, m, prm);
                        cmd.Execute(args);
                        e.Handled = true;
                    }
                }
            };
        }

        /// <inheritdoc/>
        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.PreviewKeyDown += onPreviewKeyDown;
        }

        /// <inheritdoc/>
        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.PreviewKeyDown -= onPreviewKeyDown;
        }
    }
}
