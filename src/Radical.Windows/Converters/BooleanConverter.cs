using System;
using System.Globalization;
using System.Windows.Data;

namespace Radical.Windows.Converters
{
    public class BooleanConverter : IValueConverter
    {
        public object TrueValue { get; set; }

        public object FalseValue { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool flag = false;
            if (value is bool)
            {
                flag = (bool)value;
            }
            else if (value is bool?)
            {
                var nullable = (bool?)value;
                flag = nullable.HasValue ? nullable.Value : false;
            }

            return flag ? TrueValue : FalseValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (Object.Equals(value, TrueValue))
            {
                return true;
            }
            else if (Object.Equals(value, FalseValue))
            {
                return false;
            }
            else
            {
                return value;
            }
        }
    }
}
