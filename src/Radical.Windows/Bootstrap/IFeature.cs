using System;

namespace Radical.Windows.Bootstrap
{
    interface IFeature
    {
        void Setup(IServiceProvider serviceProvider, BootstrapConventions bootstrapConventions, BootstrapConfiguration bootstrapConfiguration);
    }
}
