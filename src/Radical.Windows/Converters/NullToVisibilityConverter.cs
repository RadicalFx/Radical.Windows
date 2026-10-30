using Radical.Conversions;
using System;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace Radical.Windows.Converters
{
    /// <summary>
    /// Converts a value to a <see cref="Visibility"/>: non-null values are visible, <c>null</c> values are <see cref="NullValue"/>.
    /// Pass <c>"true"</c> as the converter parameter to invert the conversion.
    /// </summary>
    [MarkupExtensionReturnType(typeof(BooleanToVisibilityConverter))]
    public class NullToVisibilityConverter : IValueConverter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NullToVisibilityConverter"/> class.
        /// </summary>
        public NullToVisibilityConverter()
        {
            NullValue = Visibility.Collapsed;
        }

        /// <summary>
        /// Gets or sets the visibility returned for <c>null</c> values. Defaults to <see cref="Visibility.Collapsed"/>.
        /// </summary>
        public Visibility NullValue
        {
            get;
            set;
        }

        /// <inheritdoc/>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (!bool.TryParse(parameter as string, out bool inverted))
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

        /// <inheritdoc/>
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}