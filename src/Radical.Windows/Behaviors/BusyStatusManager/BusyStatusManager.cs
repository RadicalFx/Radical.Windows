using System;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace Radical.Windows.Behaviors
{
    /// <summary>
    /// Exposes attached properties to show busy content as an adorner, and disable the
    /// element, while the element status is <see cref="BusyStatus.Busy"/>.
    /// </summary>
    public static class BusyStatusManager
    {
        internal static ContentPresenter WrapUserContent(object userContent)
        {
            ContentPresenter userContentPresenter;
            if (userContent is string text)
            {
                userContentPresenter = new ContentPresenter()
                {
                    Content = new TextBlock()
                    {
                        FontStyle = FontStyles.Italic,
                        Text = text,
                        VerticalAlignment = VerticalAlignment.Center,
                        HorizontalAlignment = HorizontalAlignment.Center,
                        TextTrimming = TextTrimming.CharacterEllipsis
                    }
                };
            }
            else
            {
                userContentPresenter = new ContentPresenter() { Content = userContent };
            }

            return userContentPresenter;
        }

        #region Attached Property: EnableMultiThreadingHost

        /// <summary>
        /// Identifies the EnableMultiThreadingHost attached property. Currently not used.
        /// </summary>
        public static readonly DependencyProperty EnableMultiThreadingHostProperty = DependencyProperty.RegisterAttached(
                                      "EnableMultiThreadingHost",
                                      typeof(bool),
                                      typeof(BusyStatusManager),
                                      new FrameworkPropertyMetadata(false /*, OnEnableMultiThreadingHostChanged */ ));


        /// <summary>
        /// Gets the EnableMultiThreadingHost value of the given object.
        /// </summary>
        /// <param name="owner">The object.</param>
        /// <returns>The EnableMultiThreadingHost value.</returns>
        public static bool GetEnableMultiThreadingHost(DependencyObject owner)
        {
            return (bool)owner.GetValue(EnableMultiThreadingHostProperty);
        }

        /// <summary>
        /// Sets the EnableMultiThreadingHost value of the given object.
        /// </summary>
        /// <param name="owner">The object.</param>
        /// <param name="value">The EnableMultiThreadingHost value.</param>
        public static void SetEnableMultiThreadingHost(DependencyObject owner, bool value)
        {
            owner.SetValue(EnableMultiThreadingHostProperty, value);
        }

        #endregion

        /// <summary>
        /// Identifies the Content attached property: the content displayed by the busy adorner.
        /// </summary>
        public static readonly DependencyProperty ContentProperty = DependencyProperty.RegisterAttached(
                                      "Content",
                                      typeof(object),
                                      typeof(BusyStatusManager),
                                      new FrameworkPropertyMetadata(null, OnPropertyChanged));


        /// <summary>
        /// Gets the content displayed while the given element is busy.
        /// </summary>
        /// <param name="control">The element.</param>
        /// <returns>The busy content.</returns>
        public static object GetContent(UIElement control)
        {
            return control.GetValue(ContentProperty);
        }

        /// <summary>
        /// Sets the content displayed while the given element is busy.
        /// </summary>
        /// <param name="control">The element.</param>
        /// <param name="value">The busy content.</param>
        public static void SetContent(UIElement control, object value)
        {
            control.SetValue(ContentProperty, value);
        }

        /// <summary>
        /// Identifies the Status attached property: the busy status of the element. Defaults to <see cref="BusyStatus.Idle"/>.
        /// </summary>
        public static readonly DependencyProperty StatusProperty = DependencyProperty.RegisterAttached(
                              "Status",
                              typeof(BusyStatus),
                              typeof(BusyStatusManager),
                              new FrameworkPropertyMetadata(BusyStatus.Idle, OnPropertyChanged));

        /// <summary>
        /// Gets the busy status of the given element.
        /// </summary>
        /// <param name="control">The element.</param>
        /// <returns>The busy status.</returns>
        public static BusyStatus GetStatus(UIElement control)
        {
            return (BusyStatus)control.GetValue(StatusProperty);
        }

        /// <summary>
        /// Sets the busy status of the given element.
        /// </summary>
        /// <param name="control">The element.</param>
        /// <param name="value">The busy status.</param>
        public static void SetStatus(UIElement control, BusyStatus value)
        {
            control.SetValue(StatusProperty, value);
        }

        static readonly DependencyProperty handledProperty = DependencyProperty.RegisterAttached(
                              "handled",
                              typeof(bool),
                              typeof(BusyStatusManager),
                              new FrameworkPropertyMetadata(false));

        static bool Gethandled(DependencyObject control)
        {
            return (bool)control.GetValue(handledProperty);
        }

        static void Sethandled(DependencyObject control, bool value)
        {
            control.SetValue(handledProperty, value);
        }

        static void HandleStatusChanged(FrameworkElement element)
        {
            var layer = AdornerLayer.GetAdornerLayer(element);
            Debug.WriteLineIf(layer == null, "BusyStatusManager: cannot find any AdornerLayer on the given element.");

            if (layer != null)
            {
                var content = GetContent(element);
                var status = GetStatus(element);
                var enableMultiThreadingHost = GetEnableMultiThreadingHost(element);

                switch (status)
                {
                    case BusyStatus.Idle:

                        element.IsEnabled = true;

                        var adorners = layer.GetAdorners(element);
                        Debug.WriteLineIf(adorners == null, "BusyStatusManager: cannot find any Adorner on the given element.");

                        if (adorners != null)
                        {
                            if (enableMultiThreadingHost)
                            {
                                var la = adorners.OfType<MultiThreadingBusyAdorner>().FirstOrDefault();
                                if (la != null)
                                {
                                    la.Teardown();
                                    layer.Remove(la);
                                }
                            }
                            else
                            {
                                var la = adorners.OfType<BusyAdorner>().FirstOrDefault();
                                if (la != null)
                                {
                                    layer.Remove(la);
                                }
                            }
                        }

                        break;

                    case BusyStatus.Busy:

                        element.IsEnabled = false;
                        if (enableMultiThreadingHost)
                        {
                            //var style = GetBusyStyle( element );
                            var ba = new MultiThreadingBusyAdorner(element, content);

                            layer.Add(ba);
                            ba.Setup();
                        }
                        else if (content != null)
                        {
                            var ba = new BusyAdorner(element, content);
                            layer.Add(ba);
                        }

                        break;

                    default:
                        throw new NotSupportedException();
                }
            }
        }

        static void OnPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!DesignTimeHelper.GetIsInDesignMode())
            {
                var control = (FrameworkElement)d;
                if (!control.IsLoaded && !Gethandled(control))
                {
                    control.Loaded += (s, rea) => HandleStatusChanged(control);
                    Sethandled(control, true);
                }
                else if (control.IsLoaded && e.Property == StatusProperty)
                {
                    HandleStatusChanged(control);
                }
                else if (control.IsLoaded && e.Property == ContentProperty)
                {
                    throw new NotSupportedException("BusyStatusManager: Content property cannot be changed at runtime.");
                }
            }
        }
    }
}
