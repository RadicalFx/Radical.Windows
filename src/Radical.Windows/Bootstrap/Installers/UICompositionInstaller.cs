using Microsoft.Extensions.DependencyInjection;
using Radical.Windows.ComponentModel;
using Radical.Windows.Bootstrap.Features;
using Radical.Windows.Regions;
using System;
using System.Collections.Generic;

namespace Radical.Windows.Bootstrap.Installers
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
