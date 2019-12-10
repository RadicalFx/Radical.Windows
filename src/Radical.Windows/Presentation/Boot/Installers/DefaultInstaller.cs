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
            assemblyScanningResults.Where(t => conventions.IsMessageHandler(t) && !conventions.IsExcluded(t))
                .Select(t => new
                {
                    Contracts = conventions.SelectMessageHandlerContracts(t),
                    Implementation = t
                })
                .ForEach(descriptor =>
                {
                    foreach (var contract in descriptor.Contracts)
                    {
                        services.AddTransient(contract, descriptor.Implementation);
                    }

                    //var entry = EntryBuilder.For(descriptor.Contracts.First())
                    //        .ImplementedBy(descriptor.Implementation);

                    //foreach (var fw in descriptor.Contracts.Skip(1))
                    //{
                    //    entry = entry.Forward(fw);
                    //}

                    //container.Register(entry);
                });

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

            if (!services.IsRegistered<IMessageBroker>())
            {
                services.AddSingleton<IMessageBroker, MessageBroker>();
            }

            if (!services.IsRegistered<IReleaseComponents>())
            {
                services.AddSingleton<IReleaseComponents, DefaultComponentReleaser>();
            }
        }
    }
}
