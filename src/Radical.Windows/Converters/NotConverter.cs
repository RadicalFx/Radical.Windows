using Radical.Validation;
using System;
using System.Windows.Data;
using System.Windows.Markup;

namespace Radical.Windows.Converters
{
    [MarkupExtensionReturnType(typeof(NotConverter))]
    [ValueConversion(typeof(bool), typeof(bool))]
    public sealed class NotConverter : AbstractSingletonConverter
    {
        //static WeakReference singleton = new WeakReference( null );

        //public override object ProvideValue( IServiceProvider serviceProvider )
        //{
        //    IValueConverter converter = ( IValueConverter )singleton.Target;
        //    if( converter == null )
        //    {
        //        converter = this;
        //        singleton.Target = converter;
        //    }

        //    return converter;
        //}

        public override object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            Ensure.That(value).Named("value")
                .IsNotNull()
                .IsTrue(v => v is bool);

            Ensure.That(targetType).Is<bool>();

            return !((bool)value);
        }

        public override object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            Ensure.That(value).Named("value").IsNotNull();
            Ensure.That(targetType).Is(typeof(bool));

            return !((bool)value);
        }
    }
}