using Radical.Linq;
using Radical.Windows.Input;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace Radical.Windows.Behaviors
{
    /// <summary>
    /// Add support to bind a property to the PasswordBox Password property.
    /// </summary>
    public static class Password
    {
        #region Attached Property: IsLoadedAttached

        static readonly DependencyProperty IsLoadedAttachedProperty = DependencyProperty.RegisterAttached(
                                      "IsLoadedAttached",
                                      typeof(bool),
                                      typeof(Password),
                                      new FrameworkPropertyMetadata(false));


        static bool GetIsLoadedAttached(DependencyObject owner)
        {
            return (bool)owner.GetValue(IsLoadedAttachedProperty);
        }

        static void SetIsLoadedAttached(DependencyObject owner, bool value)
        {
            owner.SetValue(IsLoadedAttachedProperty, value);
        }

        #endregion

        #region Attached Property: Command

        /// <summary>
        /// Identifies the Command attached property: the command executed, with a <see cref="PasswordBoxCommandArgs"/>
        /// parameter, if the pressed key matches one of the command gestures, or is Enter when the command has no gestures.
        /// </summary>
        public static readonly DependencyProperty CommandProperty = DependencyProperty.RegisterAttached(
                                      "Command",
                                      typeof(ICommand),
                                      typeof(Password),
                                      new FrameworkPropertyMetadata(null, OnCommandChanged));


        /// <summary>
        /// Gets the command of the given password box.
        /// </summary>
        /// <param name="owner">The password box.</param>
        /// <returns>The command.</returns>
        public static ICommand GetCommand(PasswordBox owner)
        {
            return (ICommand)owner.GetValue(CommandProperty);
        }

        /// <summary>
        /// Sets the command of the given password box.
        /// </summary>
        /// <param name="owner">The password box.</param>
        /// <param name="value">The command.</param>
        public static void SetCommand(PasswordBox owner, ICommand value)
        {
            owner.SetValue(CommandProperty, value);
        }

        #endregion

        private static void OnCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!GetIsLoadedAttached(d))
            {
                ((PasswordBox)d).Loaded += onLoaded;

                SetIsLoadedAttached(d, true);
            }
        }

        #region Attached Property: CommandParameter

        /// <summary>
        /// Identifies the CommandParameter attached property: the parameter passed to the command, wrapped in a <see cref="PasswordBoxCommandArgs"/>.
        /// </summary>
        public static readonly DependencyProperty CommandParameterProperty = DependencyProperty.RegisterAttached(
                                      "CommandParameter",
                                      typeof(object),
                                      typeof(Password),
                                      new FrameworkPropertyMetadata(null, OnCommandParameterChanged));


        /// <summary>
        /// Gets the command parameter of the given password box.
        /// </summary>
        /// <param name="owner">The password box.</param>
        /// <returns>The command parameter.</returns>
        public static object GetCommandParameter(PasswordBox owner)
        {
            return (object)owner.GetValue(CommandParameterProperty);
        }

        /// <summary>
        /// Sets the command parameter of the given password box.
        /// </summary>
        /// <param name="owner">The password box.</param>
        /// <param name="value">The command parameter.</param>
        public static void SetCommandParameter(PasswordBox owner, object value)
        {
            owner.SetValue(CommandParameterProperty, value);
        }

        #endregion

        private static void OnCommandParameterChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!GetIsLoadedAttached(d))
            {
                ((PasswordBox)d).Loaded += onLoaded;

                SetIsLoadedAttached(d, true);
            }
        }

        #region Attached Property: Text

        /// <summary>
        /// The password Text attached property.
        /// </summary>
        public static readonly DependencyProperty TextProperty = DependencyProperty.RegisterAttached
        (
            "Text",
            typeof(string),
            typeof(Password),
            new FrameworkPropertyMetadata(string.Empty, OnTextChanged)
            {
                BindsTwoWayByDefault = true,
                DefaultUpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
            }
        );

        /// <summary>
        /// Gets the password text.
        /// </summary>
        /// <param name="passwordBox">The password box.</param>
        /// <returns>The password value.</returns>
        public static string GetText(PasswordBox passwordBox)
        {
            return (string)passwordBox.GetValue(TextProperty);
        }

        /// <summary>
        /// Sets the password text.
        /// </summary>
        /// <param name="passwordBox">The password box.</param>
        /// <param name="value">The password value.</param>
        public static void SetText(PasswordBox passwordBox, string value)
        {
            passwordBox.SetValue(TextProperty, value);
        }

        #endregion

        static void OnTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!GetIsLoadedAttached(d))
            {
                ((PasswordBox)d).Loaded += onLoaded;

                SetIsLoadedAttached(d, true);
            }
            else
            {
                var box = (PasswordBox)d;
                var newPassword = (string)e.NewValue;

                SetPasswordOnPasswordBox(box, newPassword);

                //if ( !Password.GetIsUpdating( box ) )
                //{
                //    var newPassword = ( String )e.NewValue;

                //    box.PasswordChanged -= onPasswordChanged;
                //    box.Password = newPassword;
                //    box.PasswordChanged += onPasswordChanged;
                //}
            }
        }

        static void SetPasswordOnPasswordBox(PasswordBox box, string newPassword)
        {
            if (!GetIsUpdating(box))
            {
                box.PasswordChanged -= onPasswordChanged;
                box.Password = newPassword;
                box.PasswordChanged += onPasswordChanged;
            }
        }

        static readonly RoutedEventHandler onLoaded;
        static readonly RoutedEventHandler onUnloaded;
        static readonly RoutedEventHandler onPasswordChanged;
        static readonly KeyEventHandler onPreviewKeyDown;

        static Password()
        {
            onLoaded = (s, e) =>
            {
                var box = (PasswordBox)s;
                box.PasswordChanged += onPasswordChanged;
                box.PreviewKeyDown += onPreviewKeyDown;

                var newPassword = GetText(box);
                SetPasswordOnPasswordBox(box, newPassword);
            };

            onUnloaded = (s, e) =>
            {
                var box = (PasswordBox)s;
                box.PasswordChanged -= onPasswordChanged;
                box.PreviewKeyDown -= onPreviewKeyDown;
            };

            onPasswordChanged = (s, e) =>
            {
                PasswordBox box = (PasswordBox)s;

                SetIsUpdating(box, true);
                SetText(box, box.Password);
                SetIsUpdating(box, false);
            };

            onPreviewKeyDown = (s, e) =>
            {
                var d = (PasswordBox)s;
                var cmd = GetCommand(d);

                if (cmd != null)
                {
                    var prm = GetCommandParameter(d);
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

        static readonly DependencyProperty IsUpdating = DependencyProperty.RegisterAttached
        (
            "IsUpdating",
            typeof(bool),
            typeof(Password),
            new PropertyMetadata(false)
        );

        static bool GetIsUpdating(DependencyObject dp)
        {
            return (bool)dp.GetValue(IsUpdating);
        }

        static void SetIsUpdating(DependencyObject dp, bool value)
        {
            dp.SetValue(IsUpdating, value);
        }

        //static void OnPasswordChanged( object sender, RoutedEventArgs e )
        //{
        //    PasswordBox box = sender as PasswordBox;

        //    Password.SetIsUpdating( box, true );
        //    Password.SetText( box, box.Password );
        //    Password.SetIsUpdating( box, false );
        //}
    }
}