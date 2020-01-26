using Microsoft.Extensions.DependencyInjection;
using Radical.ComponentModel;
using Radical.Windows.ComponentModel;
using Radical.Windows.Bootstrap.Features;
using Radical.Windows.Threading;
using System;
using System.Collections.Generic;
using System.Windows;

namespace Radical.Windows.Bootstrap.Installers
{
    class DefaultInstaller : IDependenciesInstaller
    {
        public void Install(BootstrapConventions conventions, IServiceCollection services, IEnumerable<Type> assemblyScanningResults)
        {
            services.AddSingleton<IFeature, Cultures>();
            services.AddSingleton<IFeature, CurrentPrincipal>();

            services.AddSingleton(container => Application.Current);
            services.AddSingleton(container => Application.Current.Dispatcher);

            if (!services.IsRegistered<IDispatcher>())
            {
                services.AddSingleton<IDispatcher, WpfDispatcher>();
            }

            if (!services.IsRegistered<IReleaseComponents>())
            {
                services.AddSingleton<IReleaseComponents, DefaultComponentReleaser>();
            }
        }
    }
}
