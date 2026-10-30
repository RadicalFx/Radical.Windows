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
    /// <summary>
    /// A behavior that allows to bind the password of a <see cref="PasswordBox"/>, and to execute a command
    /// if the pressed key matches one of the command gestures, or is Enter when the command has no gestures.
    /// </summary>
    public class PasswordBoxBehavior : RadicalBehavior<PasswordBox>, ICommandSource
    {
        #region Dependency Property: Text

        /// <summary>
        /// Identifies the <see cref="Text"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty TextProperty = DependencyProperty.Register(
            "Text",
            typeof(string),
            typeof(PasswordBoxBehavior),
            new FrameworkPropertyMetadata(null, (s, e) => ((PasswordBoxBehavior)s).OnTextChanged((string)e.NewValue))
            {
                DefaultUpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                BindsTwoWayByDefault = true,
            });

        /// <summary>
        /// Gets or sets the password. Binds two-way by default.
        /// </summary>
        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        #endregion

        #region Dependency Property: Command

        /// <summary>
        /// Identifies the <see cref="Command"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CommandProperty = DependencyProperty.Register(
            "Command",
            typeof(ICommand),
            typeof(PasswordBoxBehavior),
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
        /// Gets or sets the parameter passed to the command, wrapped in a <see cref="PasswordBoxCommandArgs"/>.
        /// </summary>
        public object CommandParameter
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the command target: the associated password box.
        /// </summary>
        public IInputElement CommandTarget
        {
            get { return AssociatedObject; }
        }

        readonly KeyEventHandler onPreviewKeyDown;
        readonly RoutedEventHandler onPasswordChanged;

        //true if going from password box to view model
        private bool isPushing;

        /// <summary>
        /// Initializes a new instance of the <see cref="PasswordBoxBehavior"/> class.
        /// </summary>
        public PasswordBoxBehavior()
        {
            onPasswordChanged = (s, e) =>
            {
                isPushing = true;

                Text = AssociatedObject.Password;

                var text = BindingOperations.GetBindingExpression(this, TextProperty);
                var tag = BindingOperations.GetBindingExpression(AssociatedObject, FrameworkElement.TagProperty);
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

                    if (((gestures.None() && e.Key == Key.Enter) || senderGestures.Any()) && cmd.CanExecute(prm))
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

        /// <inheritdoc/>
        protected override void OnAttached()
        {
            base.OnAttached();

            AssociatedObject.PasswordChanged += onPasswordChanged;
            AssociatedObject.PreviewKeyDown += onPreviewKeyDown;

            BindingOperations.SetBinding(AssociatedObject, FrameworkElement.TagProperty, new Binding()
            {
                Source = this,
                Path = new PropertyPath(TextProperty.Name),
                //Mode = BindingMode.OneWay
            });
        }

        /// <inheritdoc/>
        protected override void OnDetaching()
        {
            AssociatedObject.PasswordChanged -= onPasswordChanged;
            AssociatedObject.PreviewKeyDown -= onPreviewKeyDown;

            base.OnDetaching();
        }
    }

    /// <summary>
    /// The parameter passed to password box commands, describing the pressed key.
    /// </summary>
    public class PasswordBoxCommandArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PasswordBoxCommandArgs"/> class.
        /// </summary>
        /// <param name="key">The pressed key.</param>
        /// <param name="modifiers">The pressed modifier keys.</param>
        /// <param name="commandParameter">The command parameter configured on the password box.</param>
        public PasswordBoxCommandArgs(System.Windows.Input.Key key, System.Windows.Input.ModifierKeys modifiers, object commandParameter)
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
        /// Gets the command parameter configured on the password box.
        /// </summary>
        public object CommandParameter { get; private set; }
    }
}
