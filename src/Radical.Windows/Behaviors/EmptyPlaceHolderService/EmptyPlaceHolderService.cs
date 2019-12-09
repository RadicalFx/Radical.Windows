using Radical.Conversions;
using Radical.Linq;
using Radical.Validation;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;

namespace Radical.Windows.Behaviors
{
    public static class EmptyPlaceHolderService
    {
        #region Attached Property: Content

        public static readonly DependencyProperty ContentProperty = DependencyProperty.RegisterAttached(
                                      "Content",
                                      typeof(object),
                                      typeof(EmptyPlaceHolderService),
                                      new FrameworkPropertyMetadata(null, OnContentChanged));


        public static object GetContent(ItemsControl owner)
        {
            return (object)owner.GetValue(ContentProperty);
        }

        public static void SetContent(ItemsControl owner, object value)
        {
            owner.SetValue(ContentProperty, value);
        }

        #endregion

        static RoutedEventHandler onLoaded;
        static RoutedEventHandler onUnloaded;
        static ItemsChangedEventHandler onItemsChanged;
        static IDictionary<ItemContainerGenerator, ItemsControl> managedItemsControls = new Dictionary<ItemContainerGenerator, ItemsControl>();

        static EmptyPlaceHolderService()
        {
            onItemsChanged = (s, e) =>
            {
                var key = (ItemContainerGenerator)s;
                ItemsControl control;
                if (managedItemsControls.TryGetValue(key, out control))
                {
                    if (control.Items.Any())
                    {
                        RemoveEmptyContent(control);
                    }
                    else
                    {
                        ShowEmptyContent(control);
                    }
                }
            };

            onUnloaded = (s, e) =>
            {
                var control = (ItemsControl)s;
                var key = control.ItemContainerGenerator;
                if (managedItemsControls.ContainsKey(key))
                {
                    managedItemsControls.Remove(key);
                }

                //Vedi il CueBanner service per la spiegazione.
                //control.Loaded -= onLoaded;
                control.Unloaded -= onUnloaded;
                key.ItemsChanged -= onItemsChanged;
            };

            onLoaded = (s, e) =>
            {
                var control = (ItemsControl)s;

                var key = control.ItemContainerGenerator;
                if (!managedItemsControls.ContainsKey(key))
                {
                    managedItemsControls.Add(key, control);

                    key.ItemsChanged += onItemsChanged;
                    control.Unloaded += onUnloaded;
                }

                if (control.Items.None())
                {
                    ShowEmptyContent(control);
                }
                else
                {
                    RemoveEmptyContent(control);
                }
            };
        }

        static void OnContentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var isDesignMode = DesignTimeHelper.GetIsInDesignMode();
            if (!isDesignMode)
            {
                Ensure.That(d.GetType()).Is<ItemsControl>();

                d.CastTo<ItemsControl>().Loaded += onLoaded;
            }
        }

        static void RemoveEmptyContent(UIElement control)
        {
            AdornerLayer layer = AdornerLayer.GetAdornerLayer(control);
            Debug.WriteLineIf(layer == null, "EmptyPlaceHolderService: cannot find any AdornerLayer");

            if (layer != null)
            {
                Adorner[] adorners = layer.GetAdorners(control);
                if (adorners != null)
                {
                    adorners.OfType<EmptyContentAdorner>()
                        .ForEach(adorner =>
                       {
                           adorner.Visibility = Visibility.Hidden;
                           layer.Remove(adorner);
                       });
                }
            }
        }

        static void ShowEmptyContent(Control control)
        {
            AdornerLayer layer = AdornerLayer.GetAdornerLayer(control);
            Debug.WriteLineIf(layer == null, "EmptyPlaceHolderService: cannot find any AdornerLayer");

            if (layer != null)
            {
                Adorner[] adorners = layer.GetAdorners(control);
                if (!(adorners != null && adorners.OfType<EmptyContentAdorner>().Any()))
                {
                    layer.Add(new EmptyContentAdorner(control, control.GetValue(ContentProperty)));
                }
            }
        }
    }
}
