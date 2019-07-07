using Radical.Validation;
using Radical.Windows.Converters;
using System;
using System.Globalization;
using System.Windows.Markup;
using System.Windows.Media.Animation;

namespace Radical.Windows.Converters
{
    [MarkupExtensionReturnType(typeof(TimeSpanToKeyTimeConverter))]
    public class TimeSpanToKeyTimeConverter : AbstractSingletonConverter
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Ensure.That(value).IsNotNull().IsTrue(o => o is TimeSpan);
            Ensure.That(targetType).IsNotNull().Is(typeof(KeyTime));

            var actual = (TimeSpan)value;
            var keyTime = KeyTime.FromTimeSpan(actual);

            return keyTime;
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Ensure.That(value).IsNotNull().IsTrue(o => o is KeyTime);
            Ensure.That(targetType).IsNotNull().Is(typeof(TimeSpan));

            var actual = (KeyTime)value;
            var timeSpan = actual.TimeSpan;

            return timeSpan;
        }
    }
}
