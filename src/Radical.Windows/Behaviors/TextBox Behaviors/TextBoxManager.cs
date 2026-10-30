using Radical.Linq;
using Radical.Windows.Input;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Radical.Windows.Behaviors
{
    /// <summary>
    /// Exposes attached properties to execute commands on key press and to
    /// automatically select the text of <see cref="TextBox"/> controls.
    /// </summary>
    public static class TextBoxManager
    {
        static readonly KeyEventHandler onPreviewKeyDown;
        static readonly RoutedEventHandler onGotFocus;
        static readonly RoutedEventHandler onLoaded;
        static readonly RoutedEventHandler onUnloaded;

        static TextBoxManager()
        {
            onPreviewKeyDown = (s, e) =>
            {
                var d = (DependencyObject)s;

                var cmd = GetCommand(d);
                var prm = GetCommandParameter(d);

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

            onLoaded = (s, e) =>
            {
                var textBox = (TextBox)s;

                textBox.PreviewKeyDown += onPreviewKeyDown;
                textBox.Unloaded += onUnloaded;
            };

            onUnloaded = (s, e) =>
            {
                var textBox = (TextBox)s;

                //Vedi il CueBannerService per i dettagli
                //textBox.Loaded -= onLoaded;

                textBox.Unloaded -= onUnloaded;
                textBox.PreviewKeyDown -= onPreviewKeyDown;
            };

            onGotFocus = (s, e) =>
            {
                var source = (s as TextBox);
                source.SelectAll();
            };
        }

        #region Attached Property: Command

        /// <summary>
        /// Identifies the Command attached property: the command executed, with a <see cref="TextBoxCommandArgs"/>
        /// parameter, if the pressed key matches one of the command gestures, or is Enter when the command has no gestures.
        /// </summary>
        public static readonly DependencyProperty CommandProperty = DependencyProperty.RegisterAttached(
                                      "Command",
                                      typeof(ICommand),
                                      typeof(TextBoxManager),
                                      new FrameworkPropertyMetadata(null, OnCommandChanged));


        /// <summary>
        /// Gets the command of the given text box.
        /// </summary>
        /// <param name="owner">The text box.</param>
        /// <returns>The command.</returns>
        public static ICommand GetCommand(DependencyObject owner)
        {
            return (ICommand)owner.GetValue(CommandProperty);
        }

        /// <summary>
        /// Sets the command of the given text box.
        /// </summary>
        /// <param name="owner">The text box.</param>
        /// <param name="value">The command.</param>
        public static void SetCommand(DependencyObject owner, ICommand value)
        {
            owner.SetValue(CommandProperty, value);
        }

        #endregion

        #region Attached Property: CommandParameter

        /// <summary>
        /// Identifies the CommandParameter attached property: the parameter passed to the command, wrapped in a <see cref="TextBoxCommandArgs"/>.
        /// </summary>
        public static readonly DependencyProperty CommandParameterProperty = DependencyProperty.RegisterAttached(
                                      "CommandParameter",
                                      typeof(object),
                                      typeof(TextBoxManager),
                                      new FrameworkPropertyMetadata(null));


        /// <summary>
        /// Gets the command parameter of the given text box.
        /// </summary>
        /// <param name="owner">The text box.</param>
        /// <returns>The command parameter.</returns>
        public static object GetCommandParameter(DependencyObject owner)
        {
            return (object)owner.GetValue(CommandParameterProperty);
        }

        /// <summary>
        /// Sets the command parameter of the given text box.
        /// </summary>
        /// <param name="owner">The text box.</param>
        /// <param name="value">The command parameter.</param>
        public static void SetCommandParameter(DependencyObject owner, object value)
        {
            owner.SetValue(CommandParameterProperty, value);
        }

        #endregion

        private static void OnCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!DesignTimeHelper.GetIsInDesignMode())
            {
                if (d is TextBox textBox)
                {
                    textBox.Loaded += onLoaded;
                }
            }
        }

        #region Attached Property: AutoSelectText

        /// <summary>
        /// Identifies the AutoSelectText attached property: when <c>true</c> the whole text is selected when the text box gets focus.
        /// </summary>
        public static readonly DependencyProperty AutoSelectTextProperty = DependencyProperty.RegisterAttached(
                                      "AutoSelectText",
                                      typeof(bool),
                                      typeof(TextBoxManager),
                                      new FrameworkPropertyMetadata(false, OnAutoSelectTextChanged));


        /// <summary>
        /// Gets a value indicating whether the text of the given text box is selected when it gets focus.
        /// </summary>
        /// <param name="owner">The text box.</param>
        /// <returns><c>true</c> if the text is automatically selected; otherwise, <c>false</c>.</returns>
        public static bool GetAutoSelectText(TextBox owner)
        {
            return (bool)owner.GetValue(AutoSelectTextProperty);
        }

        /// <summary>
        /// Sets a value indicating whether the text of the given text box is selected when it gets focus.
        /// </summary>
        /// <param name="owner">The text box.</param>
        /// <param name="value"><c>true</c> to automatically select the text; otherwise, <c>false</c>.</param>
        public static void SetAutoSelectText(TextBox owner, bool value)
        {
            owner.SetValue(AutoSelectTextProperty, value);
        }

        #endregion

        private static void OnAutoSelectTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var source = (d as TextBox);
            if (source != null)
            {
                if ((bool)e.NewValue)
                {
                    source.GotFocus += onGotFocus;
                }
                else
                {
                    source.GotFocus -= onGotFocus;
                }
            }
        }
    }
}