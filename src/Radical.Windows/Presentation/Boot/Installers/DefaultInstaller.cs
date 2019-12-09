using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Radical.Windows.Presentation.Boot.Installers
{
    class DefaultInstaller : IDependenciesInstaller
    {
        public void Install(BootstrapConventions conventions, IServiceCollection services, IEnumerable<Type> assemblyScanningResults)
        {
            //        allTypes.Where(t => conventions.IsMessageHandler(t) && !conventions.IsExcluded(t))
            //            .Select(t => new
            //            {
            //                Contracts = conventions.SelectMessageHandlerContracts(t),
            //                Implementation = t
            //            })
            //            .ForEach(descriptor =>
            //            {
            //                var entry = EntryBuilder.For(descriptor.Contracts.First())
            //                        .ImplementedBy(descriptor.Implementation);

            //                foreach (var fw in descriptor.Contracts.Skip(1))
            //                {
            //                    entry = entry.Forward(fw);
            //                }

            //                container.Register(entry);
            //            });

            //        container.Register
            //        (
            //            EntryBuilder.For<TraceSource>()
            //                .UsingFactory(() =>
            //                {
            //                    var name = ConfigurationManager
            //                        .AppSettings["radical/windows/presentation/diagnostics/applicationTraceSourceName"]
            //                        .Return(s => s, "default");

            //                    return new TraceSource(name);
            //                })
            //                .WithLifestyle(Lifestyle.Singleton)
            //        );


            //        container.Register(
            //            EntryBuilder.For<Dispatcher>()
            //                .UsingFactory(() => Application.Current.Dispatcher)
            //                .WithLifestyle(Lifestyle.Singleton)
            //        );

            //        container.Register(
            //            EntryBuilder.For<IDispatcher>()
            //                .ImplementedBy<WpfDispatcher>()
            //                .WithLifestyle(Lifestyle.Singleton)
            //                .Overridable()
            //        );

            //        container.Register(
            //            EntryBuilder.For<Application>()
            //                .UsingFactory(() => Application.Current)
            //                .WithLifestyle(Lifestyle.Singleton)
            //        );

            //        container.Register(
            //            EntryBuilder.For<IMessageBroker>()
            //                .ImplementedBy<MessageBroker>()
            //                .WithLifestyle(Lifestyle.Singleton)
            //                .Overridable()
            //        );

            //        container.Register(
            //            EntryBuilder.For<IReleaseComponents>()
            //                .ImplementedBy<PuzzleComponentReleaser>()
            //                .WithLifestyle(Lifestyle.Singleton)
            //                .Overridable()
            //        );
        }
    }
}
