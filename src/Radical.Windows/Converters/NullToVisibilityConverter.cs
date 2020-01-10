using Radical.Conversions;
using System;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace Radical.Windows.Converters
{
    [MarkupExtensionReturnType(typeof(BooleanToVisibilityConverter))]
    public class NullToVisibilityConverter : IValueConverter
    {
        public NullToVisibilityConverter()
        {
            NullValue = Visibility.Collapsed;
        }

        public Visibility NullValue
        {
            get;
            set;
        }

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (!bool.TryParse(parameter.As<string>(), out bool inverted))
            {
                inverted = false;
            }

            //bool flag = false;
            //if( value is bool )
            //{
            //    flag = ( bool )value;
            //}
            //else if( value is bool? )
            //{
            //    var nullable = ( bool? )value;
            //    flag = nullable.HasValue ? nullable.Value : false;
            //}

            if (inverted)
            {
                return (value == null ? Visibility.Visible : NullValue);
            }
            else
            {
                return (value == null ? NullValue : Visibility.Visible);
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}