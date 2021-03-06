﻿using Radical.Reflection;
using Radical.Validation;
using System;
using System.Windows.Markup;

namespace Radical.Windows.Converters
{
    [MarkupExtensionReturnType(typeof(EnumDescriptionConverter))]
    public sealed class EnumDescriptionConverter : AbstractSingletonConverter
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
            if (value != null && targetType == typeof(string))
            {
                Ensure.That(value)
                    .WithMessage("Supplied value is not an Enum value.")
                    .IsTrue(v => v.GetType().Is<Enum>())
                    .WithMessage("Supplied enum does not define the required EnumItemDescriptionAttribute.")
                    .IsTrue(v => ((Enum)v).IsDescriptionAttributeDefined());

                return ((Enum)value).GetDescription();
            }

            return null;
        }

        public override object ConvertBack(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new System.NotImplementedException();
        }
    }
}