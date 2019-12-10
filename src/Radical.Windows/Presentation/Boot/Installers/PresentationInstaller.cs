using Microsoft.Extensions.DependencyInjection;
using Radical.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Radical.Windows.Presentation.Boot.Installers
{
    class PresentationInstaller : IDependenciesInstaller
    {
        public void Install(BootstrapConventions conventions, IServiceCollection services, IEnumerable<Type> assemblyScanningResults)
        {
            assemblyScanningResults.Where(t => conventions.IsViewModel(t) && !conventions.IsExcluded(t))
                .Select(t =>
                {
                    var contracts = conventions.SelectViewModelContracts(t);

                    return new
                    {
                        Contracts = conventions.SelectViewModelContracts(t),
                        Implementation = t,
                        Lifetime = conventions.IsShellViewModel(contracts, t) ?
                            ServiceLifetime.Singleton :
                            ServiceLifetime.Transient
                    };
                })
                .ForEach(descriptor =>
                {
                    foreach (var contract in descriptor.Contracts)
                    {
                        services.Add(new ServiceDescriptor(contract, descriptor.Implementation, descriptor.Lifetime));
                    }

                    //var builder = EntryBuilder.For(descriptor.Contracts.First())
                    //    .WithLifestyle(descriptor.Lifestyle);

                    //foreach (var c in descriptor.Contracts.Skip(1))
                    //{
                    //    builder = builder.Forward(c);
                    //}

                    //container.Register(builder);
                });

            assemblyScanningResults.Where(t => conventions.IsView(t) && !conventions.IsExcluded(t))
                .Select(t =>
                {
                    var contracts = conventions.SelectViewContracts(t);

                    return new
                    {
                        Contracts = contracts,
                        Implementation = t,
                        Lifetime = conventions.IsShellView(contracts, t) ?
                            ServiceLifetime.Singleton :
                            ServiceLifetime.Transient
                    };
                })
                .ForEach(descriptor =>
                {
                    foreach (var contract in descriptor.Contracts)
                    {
                        services.Add(new ServiceDescriptor(contract, descriptor.Implementation, descriptor.Lifetime));
                    }

                    //var builder = EntryBuilder.For(descriptor.Contracts.First())
                    //    .WithLifestyle(descriptor.Lifestyle);

                    //foreach (var c in descriptor.Contracts.Skip(1))
                    //{
                    //    builder = builder.Forward(c);
                    //}

                    //container.Register(builder);
                });
        }
    }
}
