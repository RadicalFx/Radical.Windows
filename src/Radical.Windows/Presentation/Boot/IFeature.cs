using System;

namespace Radical.Windows.Presentation.Boot
{
    interface IFeature
    {
        void Setup(IServiceProvider serviceProvider, ApplicationSettings applicationSettings);
    }
}
