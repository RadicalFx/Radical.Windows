using Microsoft.Extensions.DependencyInjection;
using Radical.Windows.Presentation.Boot.Features;
using Radical.Windows.Presentation.ComponentModel;
using Radical.Windows.Presentation.Regions;
using System;
using System.Collections.Generic;

namespace Radical.Windows.Presentation.Boot.Installers
{
    class UICompositionInstaller : IDependenciesInstaller
    {
        public void Install(BootstrapConventions conventions, IServiceCollection services, IEnumerable<Type> assemblyScanningResults)
        {
            services.AddSingleton<IFeature, UIComposition>();

            if (!services.IsRegistered<IRegionManagerFactory>())
            {
                services.AddSingleton<IRegionManagerFactory, RegionManagerFactory>();
            }

            if (!services.IsRegistered<IRegionService>())
            {
                services.AddSingleton<IRegionService, RegionService>();
            }

            if (!services.IsRegistered<IRegionManager>())
            {
                services.AddTransient<IRegionManager, RegionManager>();
            }
        }
    }
}
