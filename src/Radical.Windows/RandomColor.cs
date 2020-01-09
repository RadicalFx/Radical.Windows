using System;
using System.Reflection;
using System.Windows.Media;

namespace Radical.Windows
{
    public sealed class RandomSolidColorBrush
    {
        readonly Random random;
        readonly PropertyInfo[] _props;
        readonly int _MaxProps;

        public RandomSolidColorBrush()
        {
            random = new Random((int)DateTime.Now.Ticks);
            _props = typeof(Brushes).GetProperties();
            _MaxProps = _props.Length;
        }

        public SolidColorBrush Next()
        {
            var number = random.Next(_MaxProps);
            return (SolidColorBrush)_props[number].GetValue(null, null);
        }
    }
}
