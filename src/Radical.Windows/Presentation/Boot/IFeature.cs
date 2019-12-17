using System;
using System.Collections.Generic;
using System.Text;

namespace Radical.Windows.Presentation.Boot
{
    interface IFeature
    {
        void Setup(IServiceProvider serviceProvider, ApplicationSettings applicationSettings);
    }
}
