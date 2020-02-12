using System;

namespace Radical.Windows.Bootstrap
{
    interface IFeature
    {
        void Setup(IServiceProvider serviceProvider, ApplicationSettings applicationSettings);
    }
}
