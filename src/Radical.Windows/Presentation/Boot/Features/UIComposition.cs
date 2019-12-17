using Radical.Windows.Presentation.ComponentModel;
using Radical.Windows.Presentation.Regions;
using System;

namespace Radical.Windows.Presentation.Boot.Features
{
    class UIComposition : IFeature
    {
        public void Setup(IServiceProvider serviceProvider, ApplicationSettings applicationSettings)
        {
            RegionService.CurrentService = serviceProvider.GetService<IRegionService>();
            RegionService.Conventions = serviceProvider.GetService<IConventionsHandler>();
        }
    }
}
