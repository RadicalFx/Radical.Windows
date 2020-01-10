using Radical.Linq;
using Radical.Windows.Input;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Radical.Windows.Behaviors
{
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

        public static readonly DependencyProperty CommandProperty = DependencyProperty.RegisterAttached(
                                      "Command",
                                      typeof(ICommand),
                                      typeof(TextBoxManager),
                                      new FrameworkPropertyMetadata(null, OnCommandChanged));


        public static ICommand GetCommand(DependencyObject owner)
        {
            return (ICommand)owner.GetValue(CommandProperty);
        }

        public static void SetCommand(DependencyObject owner, ICommand value)
        {
            owner.SetValue(CommandProperty, value);
        }

        #endregion

        #region Attached Property: CommandParameter

        public static readonly DependencyProperty CommandParameterProperty = DependencyProperty.RegisterAttached(
                                      "CommandParameter",
                                      typeof(object),
                                      typeof(TextBoxManager),
                                      new FrameworkPropertyMetadata(null));


        public static object GetCommandParameter(DependencyObject owner)
        {
            return (object)owner.GetValue(CommandParameterProperty);
        }

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

        public static readonly DependencyProperty AutoSelectTextProperty = DependencyProperty.RegisterAttached(
                                      "AutoSelectText",
                                      typeof(bool),
                                      typeof(TextBoxManager),
                                      new FrameworkPropertyMetadata(false, OnAutoSelectTextChanged));


        public static bool GetAutoSelectText(TextBox owner)
        {
            return (bool)owner.GetValue(AutoSelectTextProperty);
        }

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