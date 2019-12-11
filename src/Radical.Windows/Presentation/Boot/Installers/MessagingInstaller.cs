﻿using Microsoft.Extensions.DependencyInjection;
using Radical.ComponentModel.Messaging;
using Radical.Linq;
using Radical.Messaging;
using Radical.Windows.Presentation.Boot.Features;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Radical.Windows.Presentation.Boot.Installers
{
    class MessagingInstaller : IDependenciesInstaller
    {
        public void Install(BootstrapConventions conventions, IServiceCollection services, IEnumerable<Type> assemblyScanningResults)
        {
            var autoSubscribeFeature = new AutoSubscribe();
            services.AddSingleton<IFeature>(autoSubscribeFeature);

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
                        services.AddTransient(descriptor.Implementation);
                    }

                    autoSubscribeFeature.Add(descriptor.Implementation, descriptor.Contracts);
                });

            if (!services.IsRegistered<IMessageBroker>())
            {
                services.AddSingleton<IMessageBroker, MessageBroker>();
            }
        }
    }
}
