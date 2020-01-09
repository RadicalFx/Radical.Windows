﻿//Copyright (c) 2008 Jason Kemp
//
//Permission is hereby granted, free of charge, to any person obtaining a copy
//of this software and associated documentation files (the "Software"), to deal
//in the Software without restriction, including without limitation the rights
//to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//copies of the Software, and to permit persons to whom the Software is
//furnished to do so, subject to the following conditions:
//
//The above copyright notice and this permission notice shall be included in
//all copies or substantial portions of the Software.
//
//THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
//THE SOFTWARE.

using System.Windows;
using System.Windows.Controls;

namespace Radical.Windows.Behaviors
{
    public sealed partial class AutoComplete
    {
        public static readonly DependencyProperty UserTextProperty = DependencyProperty.RegisterAttached("UserText",
                typeof(object),
                typeof(AutoComplete),
                new FrameworkPropertyMetadata(null, OnUserTextPropertyChanged)
                {
                    BindsTwoWayByDefault = true
                });

        public static readonly DependencyProperty SourceProperty = DependencyProperty.RegisterAttached("Source",
                typeof(object),
                typeof(AutoComplete),
                new FrameworkPropertyMetadata(null, OnSourcePropertyChanged));

        public static readonly DependencyProperty FilterPathProperty = DependencyProperty.RegisterAttached("FilterPath",
                typeof(AutoCompleteFilterPathCollection),
                typeof(AutoComplete),
                new FrameworkPropertyMetadata(null));

        public static readonly DependencyProperty ChoosenItemProperty = DependencyProperty.RegisterAttached("ChoosenItem",
                typeof(object),
                typeof(AutoComplete),
                new FrameworkPropertyMetadata(null)
                {
                    BindsTwoWayByDefault = true
                });

        private static readonly DependencyPropertyKey AutoCompleteInstancePropertyKey = DependencyProperty.RegisterAttachedReadOnly(
                "AutoCompleteInstance",
                typeof(AutoComplete),
                typeof(AutoComplete),
                new FrameworkPropertyMetadata(null));

        private static readonly DependencyProperty ItemTemplateProperty = DependencyProperty.RegisterAttached("ItemTemplate",
               typeof(DataTemplate),
               typeof(AutoComplete),
               new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnItemTemplatePropertyChanged)));

        private static readonly DependencyProperty AutoCompleteInstance = AutoCompleteInstancePropertyKey.DependencyProperty;

        #region Attached Property: ImplicitItemsFilter

        public static readonly DependencyProperty ImplicitItemsFilterProperty = DependencyProperty.RegisterAttached(
                                      "ImplicitItemsFilter",
                                      typeof(ImplicitItemsFilter),
                                      typeof(AutoComplete),
                                      new FrameworkPropertyMetadata(ImplicitItemsFilter.Enabled));


        public static ImplicitItemsFilter GetImplicitItemsFilter(DependencyObject owner)
        {
            return (ImplicitItemsFilter)owner.GetValue(ImplicitItemsFilterProperty);
        }

        public static void SetImplicitItemsFilter(DependencyObject owner, ImplicitItemsFilter value)
        {
            owner.SetValue(ImplicitItemsFilterProperty, value);
        }

        #endregion

        public static object GetUserText(DependencyObject d)
        {
            return d.GetValue(UserTextProperty);
        }

        public static void SetUserText(DependencyObject d, object value)
        {
            d.SetValue(UserTextProperty, value);
        }

        public static object GetSource(DependencyObject d)
        {
            return d.GetValue(SourceProperty);
        }

        public static void SetSource(DependencyObject d, object value)
        {
            d.SetValue(SourceProperty, value);
        }

        public static AutoCompleteFilterPathCollection GetFilterPath(DependencyObject d)
        {
            return (AutoCompleteFilterPathCollection)d.GetValue(FilterPathProperty);
        }

        public static void SetFilterPath(DependencyObject d, AutoCompleteFilterPathCollection value)
        {
            d.SetValue(FilterPathProperty, value);
        }

        public static DataTemplate GetItemTemplate(DependencyObject d)
        {
            return (DataTemplate)d.GetValue(ItemTemplateProperty);
        }

        public static void SetItemTemplate(DependencyObject d, object value)
        {
            d.SetValue(ItemTemplateProperty, value);
        }

        public static object GetChoosenItem(DependencyObject d)
        {
            return (object)d.GetValue(ChoosenItemProperty);
        }

        public static void SetChoosenItem(DependencyObject d, object value)
        {
            d.SetValue(ChoosenItemProperty, value);
        }

        private static void OnUserTextPropertyChanged(DependencyObject d,
                                            DependencyPropertyChangedEventArgs e)
        {

        }

        private static void OnSourcePropertyChanged(DependencyObject d,
                                            DependencyPropertyChangedEventArgs e)
        {
            AutoComplete ac = EnsureInstance((Control)d);
            ac.ViewSource.Source = e.NewValue;
        }

        private static void OnItemTemplatePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            AutoComplete ac = EnsureInstance((Control)d);
            ac.ListBox.ItemTemplate = (DataTemplate)e.NewValue;
        }

        private static AutoComplete EnsureInstance(Control ctrl)
        {
            var ac = (AutoComplete)ctrl.GetValue(AutoComplete.AutoCompleteInstance);
            if (ac == null)
            {
                ac = new AutoComplete(ctrl);
                ctrl.SetValue(AutoCompleteInstancePropertyKey, ac);
            }

            return ac;
        }
    }
}
