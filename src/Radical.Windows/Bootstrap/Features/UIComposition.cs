using Radical.Windows.ComponentModel;
using Radical.Windows.Regions;
using System;

namespace Radical.Windows.Bootstrap.Features
{
    class UIComposition : IFeature
    {
        public void Setup(IServiceProvider serviceProvider, BootstrapConventions bootstrapConventions, BootstrapConfiguration bootstrapConfiguration)
        {
            RegionService.CurrentService = serviceProvider.GetService<IRegionService>();
            RegionService.Conventions = serviceProvider.GetService<IConventionsHandler>();
        }
    }
}
