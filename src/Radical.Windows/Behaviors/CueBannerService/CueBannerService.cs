using Radical.Linq;
using System;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;

namespace Radical.Windows.Behaviors
{
    public static class CueBannerService
    {
        #region CueBanner Attached Property

        public static readonly DependencyProperty CueBannerProperty = DependencyProperty.RegisterAttached(
                                      "CueBanner",
                                      typeof(object),
                                      typeof(CueBannerService),
                                      new FrameworkPropertyMetadata(null, OnCueBannerPropertyChanged));

        public static object GetCueBanner(TextBoxBase control)
        {
            return control.GetValue(CueBannerProperty);
        }

        public static void SetCueBanner(TextBoxBase control, object value)
        {
            control.SetValue(CueBannerProperty, value);
        }

        #endregion

        #region Attached Property: PasswordCueBanner

        public static readonly DependencyProperty PasswordCueBannerProperty = DependencyProperty.RegisterAttached(
                                      "PasswordCueBanner",
                                      typeof(object),
                                      typeof(CueBannerService),
                                      new FrameworkPropertyMetadata(null, OnCueBannerPropertyChanged));


        public static object GetPasswordCueBanner(PasswordBox owner)
        {
            return (object)owner.GetValue(PasswordCueBannerProperty);
        }

        public static void SetPasswordCueBanner(PasswordBox owner, object value)
        {
            owner.SetValue(PasswordCueBannerProperty, value);
        }

        #endregion

        static readonly DependencyPropertyChangedEventHandler onVisibleChanged = null;

        static CueBannerService()
        {
            onVisibleChanged = (s, e) =>
            {
                HandleShowRequest((UIElement)s);
            };
        }

        static readonly RoutedEventHandler onLoaded = (s, e) =>
        {
            var control = (UIElement)s;

            control.AddHandler(FrameworkElement.UnloadedEvent, onUnloaded, true);
            control.AddHandler(TextBoxBase.TextChangedEvent, onTextChanged, true);

            control.AddHandler(PasswordBox.PasswordChangedEvent, onPasswordChanged, true);

            control.IsVisibleChanged += onVisibleChanged;

            HandleShowRequest(control);
        };

        static void HandleShowRequest(UIElement control)
        {
            if (control is TextBoxBase tbb)
            {
                if (tbb.Visibility == Visibility.Visible)
                {
                    if (ShouldShowCueBanner(tbb))
                    {
                        ShowCueBanner(tbb, GetCueBanner(tbb));
                    }
                }
                else
                {
                    RemoveCueBanner(tbb);
                }
            }
            else if (control is PasswordBox pb)
            {
                if (pb.Visibility == Visibility.Visible)
                {
                    if (ShouldShowCueBanner(pb))
                    {
                        ShowCueBanner(pb, GetPasswordCueBanner(pb));
                    }
                }
                else
                {
                    RemoveCueBanner(pb);
                }
            }
        }

        static readonly RoutedEventHandler onUnloaded = (s, e) =>
        {
            var control = (UIElement)s;

            /*
             * TODO: capire perchè...
             * 
             * control.Loaded -= onLoaded;
             * 
             * Non dobbiamo sganciarci dall'evento, un elemento Wpf viene
             * unloaded anche quando viene rimosso da un VisualTree se ci 
             * sganciamo da Loaded non sapremo mai che la textBox è tornata
             * in un visual tree e quindi, ad esempio in un tabControl, ci
             * perdiamo tutte le funzionalità.
             */
            //control.Unloaded -= onUnloaded;
            //control.TextChanged -= onTextChanged;
            control.RemoveHandler(FrameworkElement.UnloadedEvent, onUnloaded);
            control.RemoveHandler(TextBoxBase.TextChangedEvent, onTextChanged);

            control.RemoveHandler(PasswordBox.PasswordChangedEvent, onPasswordChanged);

            control.IsVisibleChanged -= onVisibleChanged;
        };

        static readonly TextChangedEventHandler onTextChanged = (s, e) =>
        {
            var c = (TextBoxBase)s;
            if (ShouldShowCueBanner(c))
            {
                ShowCueBanner(c, GetCueBanner(c));
            }
            else
            {
                RemoveCueBanner(c);
            }
        };

        static readonly RoutedEventHandler onPasswordChanged = (s, e) =>
        {
            var c = (PasswordBox)s;
            if (ShouldShowCueBanner(c))
            {
                ShowCueBanner(c, GetPasswordCueBanner(c));
            }
            else
            {
                RemoveCueBanner(c);
            }
        };

        static void OnCueBannerPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var isDesignMode = DesignTimeHelper.GetIsInDesignMode();
            if (!isDesignMode)
            {
                var control = (UIElement)sender;
                control.AddHandler(FrameworkElement.LoadedEvent, onLoaded, true);
            }
        }

        static void RemoveCueBanner(FrameworkElement control)
        {
            AdornerLayer layer = AdornerLayer.GetAdornerLayer(control);
            Debug.WriteLineIf(layer == null, "CueBannerService: cannot find any AdornerLayer.");

            if (layer != null)
            {
                var adorners = layer.GetAdorners(control);
                Debug.WriteLineIf(adorners == null, "CueBannerService: cannot find any Adorner on the given control.");

                if (adorners != null)
                {
                    adorners.OfType<CueBannerAdorner>()
                        .ForEach(adorner =>
                       {
                           adorner.Visibility = Visibility.Hidden;
                           layer.Remove(adorner);
                       });
                }
            }
        }

        static void ShowCueBanner(FrameworkElement control, object content)
        {
            var layer = AdornerLayer.GetAdornerLayer(control);
            Debug.WriteLineIf(layer == null, "CueBannerService: cannot find any AdornerLayer.");

            if (layer != null)
            {
                layer.Add(new CueBannerAdorner(control, content));
            }
        }

        static bool ShouldShowCueBanner(TextBoxBase c)
        {
            var value = c.GetValue(TextBox.TextProperty) as string;
            var isVisible = Convert.ToBoolean(c.GetValue(UIElement.IsVisibleProperty));

            return isVisible && string.IsNullOrEmpty(value);
        }

        static bool ShouldShowCueBanner(PasswordBox c)
        {
            var value = c.Password;
            var isVisible = c.IsVisible;

            return isVisible && string.IsNullOrEmpty(value);
        }
    }
}