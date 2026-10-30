using System;
using System.Globalization;
using System.Windows.Data;

namespace Radical.Windows.Converters
{
    /// <summary>
    /// Converts a <see cref="bool"/> to <see cref="TrueValue"/> or <see cref="FalseValue"/>, and back.
    /// </summary>
    public class BooleanConverter : IValueConverter
    {
        /// <summary>
        /// Gets or sets the value returned when the converted value is <c>true</c>.
        /// </summary>
        public object TrueValue { get; set; }

        /// <summary>
        /// Gets or sets the value returned when the converted value is <c>false</c> or <c>null</c>.
        /// </summary>
        public object FalseValue { get; set; }

        /// <inheritdoc/>
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

        /// <inheritdoc/>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (object.Equals(value, TrueValue))
            {
                return true;
            }
            else if (object.Equals(value, FalseValue))
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
