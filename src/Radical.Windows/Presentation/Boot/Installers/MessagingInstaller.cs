using Microsoft.Extensions.DependencyInjection;
using Radical.ComponentModel.Messaging;
using Radical.Linq;
using Radical.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Radical.Windows.Presentation.Boot.Installers
{
    class MessagingInstaller : IDependenciesInstaller
    {
        public void Install(BootstrapConventions conventions, IServiceCollection services, IEnumerable<Type> assemblyScanningResults)
        {
            var collector = new Collector();
            services.AddSingleton(collector);

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

                    collector.Entries.Add(new Entry() 
                    {
                        Implementation = descriptor.Implementation,
                        Contracts = descriptor.Contracts
                    });

                    //var entry = EntryBuilder.For(descriptor.Contracts.First())
                    //        .ImplementedBy(descriptor.Implementation);

                    //foreach (var fw in descriptor.Contracts.Skip(1))
                    //{
                    //    entry = entry.Forward(fw);
                    //}

                    //container.Register(entry);
                });

            if (!services.IsRegistered<IMessageBroker>())
            {
                services.AddSingleton<IMessageBroker, MessageBroker>();
            }
        }
    }

    class Collector 
    {
        public List<Entry> Entries { get; set; } = new List<Entry>();
    }

    public class Entry
    {
        public Type Implementation { get; set; }
        public IEnumerable<Type> Contracts { get; set; }
    }
}
