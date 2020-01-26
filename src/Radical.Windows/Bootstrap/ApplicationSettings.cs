using System;
using System.Globalization;

namespace Radical.Windows.Bootstrap
{
    class ApplicationSettings
    {
        public Func<CultureInfo> CurrentCultureHandler { get; set; } = () => CultureInfo.CurrentCulture;
        public Func<CultureInfo> CurrentUICultureHandler { get; set; } = () => CultureInfo.CurrentUICulture;
    }
}
