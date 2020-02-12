//Copyright (c) 2008 Jason Kemp
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
using System;
using System.ComponentModel;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

namespace Radical.Windows.Behaviors
{
    partial class AutoComplete
    {
        public interface ICanRepresentMyself
        {
            string AsString();
        }

        public interface IHaveAnOpinionOnFilter
        {
            bool Match(string userText);
        }

        private readonly ControlUnderAutoComplete controlUnderAutocomplete;
        private readonly CollectionViewSource viewSource;

        private bool iteratingListItems;
        private bool deletingText;
        private string rememberedText;
        private readonly Popup autoCompletePopup;

        /// <summary>
        /// Initializes a new instance of the <see cref="AutoComplete"/> class.
        /// </summary>
        public AutoComplete(Control value)
        {
            InitializeComponent();

            controlUnderAutocomplete = ControlUnderAutoComplete.Create(value);

            viewSource = controlUnderAutocomplete.GetViewSource((Style)this[controlUnderAutocomplete.StyleKey]);
            viewSource.Filter += OnCollectionViewSourceFilter;

            controlUnderAutocomplete.Control.SetValue(FrameworkElement.StyleProperty, this[controlUnderAutocomplete.StyleKey]);
            controlUnderAutocomplete.Control.ApplyTemplate();

            autoCompletePopup = (Popup)controlUnderAutocomplete.Control.Template.FindName("autoCompletePopup", controlUnderAutocomplete.Control);
            _listBox = (ListBox)controlUnderAutocomplete.Control.Template.FindName("autoCompleteListBox", controlUnderAutocomplete.Control);

            var b = new Binding("ActualWidth")
            {
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                Source = controlUnderAutocomplete.Control
            };

            ListBox.SetBinding(FrameworkElement.MinWidthProperty, b);
            ListBox.PreviewMouseDown += OnListBoxPreviewMouseDown;

            controlUnderAutocomplete.Control.AddHandler(TextBoxBase.TextChangedEvent, new TextChangedEventHandler(OnTextBoxTextChanged));
            controlUnderAutocomplete.Control.LostFocus += OnTextBoxLostFocus;
            controlUnderAutocomplete.Control.PreviewKeyUp += OnTextBoxPreviewKeyUp;
            controlUnderAutocomplete.Control.PreviewKeyDown += OnTextBoxPreviewKeyDown;
        }

        internal CollectionViewSource ViewSource
        {
            get { return viewSource; }
        }

        private readonly ListBox _listBox;
        private ListBox ListBox
        {
            get { return _listBox; }
        }

        private void OnCollectionViewSourceFilter(object sender, FilterEventArgs e)
        {
            if (GetImplicitItemsFilter(controlUnderAutocomplete.Control) == ImplicitItemsFilter.Enabled)
            {
                AutoCompleteFilterPathCollection filterPaths = GetAutoCompleteFilterProperty();
                if (e.Item is AutoComplete.IHaveAnOpinionOnFilter iho)
                {
                    e.Accepted = iho.Match(controlUnderAutocomplete.Text);
                }
                else if (filterPaths != null)
                {
                    Type t = e.Item.GetType();
                    foreach (string autoCompleteProperty in filterPaths)
                    {
                        PropertyInfo info = t.GetProperty(autoCompleteProperty);
                        object value = info.GetValue(e.Item, null);
                        if (TextBoxStartsWith(value))
                        {
                            e.Accepted = true;
                            return;
                        }
                    }
                    e.Accepted = false;
                }
                else
                {
                    e.Accepted = TextBoxStartsWith(e.Item);
                }
            }
            else
            {
                e.Accepted = true;
            }
        }

        private bool TextBoxStartsWith(object value)
        {
            return value != null && value.ToString().StartsWith(controlUnderAutocomplete.Text, StringComparison.CurrentCultureIgnoreCase);
        }

        private AutoCompleteFilterPathCollection GetAutoCompleteFilterProperty()
        {
            if (GetFilterPath(controlUnderAutocomplete.Control) != null)
                return GetFilterPath(controlUnderAutocomplete.Control);
            return null;
        }

        bool ignoreOnTextBoxTextChanged = false;

        private void OnTextBoxTextChanged(object sender, TextChangedEventArgs e)
        {
            if (!ignoreOnTextBoxTextChanged)
            {
                SetUserText(controlUnderAutocomplete.Control, controlUnderAutocomplete.Text);

                if (controlUnderAutocomplete.Text == "")
                {
                    autoCompletePopup.IsOpen = false;
                    return;
                }
                if (!iteratingListItems && controlUnderAutocomplete.Text != "")
                {
                    ICollectionView v = viewSource.View;
                    v.Refresh();
                    if (v.IsEmpty)
                    {
                        autoCompletePopup.IsOpen = false;
                    }
                    else
                    {
                        autoCompletePopup.IsOpen = true;

                        if (v.CurrentItem == null)
                        {
                            v.MoveCurrentToFirst();
                        }

                        if (!deletingText && v.CurrentItem != null)
                        {
                            var item = GetTextForTextBox(v.CurrentItem);

                            //var userText = this.controlUnderAutocomplete.Text;
                            //var diff = item.Remove( 0, userText.Length );
                            //var firstAppendedCharIndex = item.IndexOf( diff );

                            ignoreOnTextBoxTextChanged = true;
                            controlUnderAutocomplete.Text = item;
                            ignoreOnTextBoxTextChanged = false;

                            //this.controlUnderAutocomplete.Select( firstAppendedCharIndex, item.Length );
                        }
                    }
                }
            }
        }

        private void OnTextBoxPreviewKeyDown(object sender, KeyEventArgs e)
        {
            deletingText = e.Key == Key.Delete || e.Key == Key.Back;

            //if( e.Key == ik.Key.Tab )
            //{
            //    this.OnItemChoosen();
            //}
        }

        private void OnTextBoxPreviewKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Up || e.Key == Key.Down)
            {
                if (rememberedText == null)
                {
                    rememberedText = controlUnderAutocomplete.Text;
                }

                iteratingListItems = true;
                var view = viewSource.View;

                if (e.Key == Key.Up)
                {
                    if (view.CurrentItem == null)
                        view.MoveCurrentToLast();
                    else
                        view.MoveCurrentToPrevious();
                }
                else
                {
                    if (view.CurrentItem == null)
                        view.MoveCurrentToFirst();
                    else
                        view.MoveCurrentToNext();
                }

                if (view.CurrentItem == null)
                {
                    controlUnderAutocomplete.Text = rememberedText;
                }
                else
                {
                    controlUnderAutocomplete.Text = GetTextForTextBox(view.CurrentItem);
                }
            }
            else
            {
                iteratingListItems = false;
                rememberedText = null;
                if (autoCompletePopup.IsOpen && (e.Key == Key.Escape || e.Key == Key.Enter))
                {
                    autoCompletePopup.IsOpen = false;
                    if (e.Key == Key.Enter)
                    {
                        controlUnderAutocomplete.SelectAll();
                        OnItemChoosen();
                    }
                }
            }
        }

        private void OnItemChoosen()
        {
            var view = viewSource.View;
            var ci = view.CurrentItem;

            SetChoosenItem(controlUnderAutocomplete.Control, ci);
        }

        private void OnTextBoxLostFocus(object sender, RoutedEventArgs e)
        {
            autoCompletePopup.IsOpen = false;
            OnItemChoosen();
        }

        private void OnListBoxPreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            var pt = e.GetPosition((IInputElement)sender);
            var hr = VisualTreeHelper.HitTest(ListBox, pt);
            if (hr != null)
            {
                var lbi = hr.VisualHit.FindParent<ListBoxItem>();
                if (lbi != null && lbi.DataContext != null)
                {
                    controlUnderAutocomplete.Text = GetTextForTextBox(lbi.DataContext);
                    autoCompletePopup.IsOpen = false;
                    controlUnderAutocomplete.SelectAll();
                    OnItemChoosen();
                }
            }
        }

        string GetTextForTextBox(object selectedItem)
        {
            var value = selectedItem is ICanRepresentMyself icrm ? icrm.AsString() : selectedItem.ToString();

            return value;
        }
    }
}
