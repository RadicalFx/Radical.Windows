using Microsoft.Extensions.DependencyInjection;
using Radical.Linq;
using Radical.Windows.Presentation.Boot.Features;
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
                });

            var regionsInjection = new AutoRegionsInjection();
            services.AddSingleton<IFeature>(regionsInjection);

            assemblyScanningResults.Where(t => conventions.IsView(t) && !conventions.IsExcluded(t))
                .Select(t =>
                {
                    var contracts = conventions.SelectViewContracts(t);

                    var regionName = conventions.GetInterestedRegionNameIfAny(t);
                    if (!string.IsNullOrWhiteSpace(regionName)) 
                    {
                        regionsInjection.Add(regionName, t);
                    }

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
                });
        }
    }
}
