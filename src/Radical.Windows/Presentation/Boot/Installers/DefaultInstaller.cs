using Microsoft.Extensions.DependencyInjection;
using Radical.ComponentModel;
using Radical.ComponentModel.Messaging;
using Radical.Linq;
using Radical.Messaging;
using Radical.Windows.Presentation.ComponentModel;
using Radical.Windows.Threading;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;

namespace Radical.Windows.Presentation.Boot.Installers
{
    class DefaultInstaller : IDependenciesInstaller
    {
        public void Install(BootstrapConventions conventions, IServiceCollection services, IEnumerable<Type> assemblyScanningResults)
        {
            services.AddSingleton(container => 
            {
                //TODO: figure out best way to do settings
                //var name = ConfigurationManager
                //            .AppSettings["radical/windows/presentation/diagnostics/applicationTraceSourceName"]
                //            .Return(s => s, "default");

                return new TraceSource("default");
            });

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
