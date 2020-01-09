using Radical.Linq;
using Radical.Windows.Input;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace Radical.Windows.Behaviors
{
    public class PasswordBoxBehavior : RadicalBehavior<PasswordBox>, ICommandSource
    {
        #region Dependency Property: Text

        public static readonly DependencyProperty TextProperty = DependencyProperty.Register(
            "Text",
            typeof(string),
            typeof(PasswordBoxBehavior),
            new FrameworkPropertyMetadata(null, (s, e) => ((PasswordBoxBehavior)s).OnTextChanged((string)e.NewValue))
            {
                DefaultUpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                BindsTwoWayByDefault = true,
            });

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        #endregion

        #region Dependency Property: Command

        public static readonly DependencyProperty CommandProperty = DependencyProperty.Register(
            "Command",
            typeof(ICommand),
            typeof(PasswordBoxBehavior),
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
        readonly RoutedEventHandler onPasswordChanged;

        //true if going from password box to view model
        private bool isPushing;

        public PasswordBoxBehavior()
        {
            onPasswordChanged = (s, e) =>
            {
                isPushing = true;

                Text = AssociatedObject.Password;

                var text = BindingOperations.GetBindingExpression(this, PasswordBoxBehavior.TextProperty);
                var tag = BindingOperations.GetBindingExpression(AssociatedObject, PasswordBox.TagProperty);
                if (text.HasError)
                {
                    System.Windows.Controls.Validation.MarkInvalid(tag, text.ValidationError);
                }
                else
                {
                    System.Windows.Controls.Validation.ClearInvalid(tag);
                }

                isPushing = false;
            };

            onPreviewKeyDown = (s, e) =>
            {
                var d = (DependencyObject)s;

                if (Command != null)
                {
                    var cmd = Command;
                    var prm = CommandParameter;

                    var gestures = cmd.GetGestures();
                    var senderGestures = gestures.Where(gesture => gesture.Matches(d, e));

                    if (((gestures.None() && e.Key == System.Windows.Input.Key.Enter) || senderGestures.Any()) && cmd.CanExecute(prm))
                    {
                        var k = e.Key;
                        var m = ModifierKeys.None;

                        if (senderGestures.Any())
                        {
                            var gesture = senderGestures.First();
                            var keygesture = gesture as KeyGesture;
                            if (keygesture != null)
                            {
                                k = keygesture.Key;
                                m = keygesture.Modifiers;
                            }
                        }

                        var args = new PasswordBoxCommandArgs(k, m, prm);
                        cmd.Execute(args);
                        e.Handled = true;
                    }
                }
            };
        }

        void OnTextChanged(string newValue)
        {
            if (!isPushing)
            {
                AssociatedObject.PasswordChanged -= onPasswordChanged;
                AssociatedObject.Password = newValue;
                AssociatedObject.PasswordChanged += onPasswordChanged;
            }
        }

        protected override void OnAttached()
        {
            base.OnAttached();

            AssociatedObject.PasswordChanged += onPasswordChanged;
            AssociatedObject.PreviewKeyDown += onPreviewKeyDown;

            BindingOperations.SetBinding(AssociatedObject, PasswordBox.TagProperty, new Binding()
            {
                Source = this,
                Path = new PropertyPath(TextProperty.Name),
                //Mode = BindingMode.OneWay
            });
        }

        protected override void OnDetaching()
        {
            AssociatedObject.PasswordChanged -= onPasswordChanged;
            AssociatedObject.PreviewKeyDown -= onPreviewKeyDown;

            base.OnDetaching();
        }
    }

    public class PasswordBoxCommandArgs : EventArgs
    {
        public PasswordBoxCommandArgs(System.Windows.Input.Key key, System.Windows.Input.ModifierKeys modifiers, object commandParameter)
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
