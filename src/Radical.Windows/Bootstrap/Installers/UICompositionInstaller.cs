using Microsoft.Extensions.DependencyInjection;
using Radical.Windows.Bootstrap.Features;
using Radical.Windows.ComponentModel;
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
            services.AddSingleton<IRegionManagerFactory, RegionManagerFactory>();
            services.AddSingleton<IRegionService, RegionService>();
            services.AddTransient<IRegionManager, RegionManager>();
        }
    }
}
