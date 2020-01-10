using Radical.Linq;
using Radical.Windows.Input;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Radical.Windows.Behaviors
{
    public class TextBoxCommandBehavior : RadicalBehavior<TextBox>, ICommandSource
    {
        #region Dependency Property: Command

        public static readonly DependencyProperty CommandProperty = DependencyProperty.Register(
            "Command",
            typeof(ICommand),
            typeof(TextBoxCommandBehavior),
            new PropertyMetadata(null));

        public ICommand Command
        {
            get { return (ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }

        #endregion

        public object CommandParameter
        {
            get;
            set;
        }

        public IInputElement CommandTarget
        {
            get { return AssociatedObject; }
        }

        readonly KeyEventHandler onPreviewKeyDown;

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

                    if (((gestures.None() && e.Key == System.Windows.Input.Key.Enter) || senderGestures.Any()))
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

        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.PreviewKeyDown += onPreviewKeyDown;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.PreviewKeyDown -= onPreviewKeyDown;
        }
    }
}
