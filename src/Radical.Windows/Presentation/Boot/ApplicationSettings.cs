using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Radical.Windows.Presentation.Boot
{
    class ApplicationSettings
    {
        public Func<CultureInfo> CurrentCultureHandler { get; set; } = () => CultureInfo.CurrentCulture;
        public Func<CultureInfo> CurrentUICultureHandler { get; set; } = () => CultureInfo.CurrentUICulture;
    }
}
