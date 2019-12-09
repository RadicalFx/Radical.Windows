using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Radical.Windows.Presentation.Boot.Installers
{
    class PresentationInstaller : IDependenciesInstaller
    {
        public void Install(BootstrapConventions conventions, IServiceCollection services, IEnumerable<Type> assemblyScanningResults)
        {
            //allTypes.Where(t => conventions.IsViewModel(t) && !conventions.IsExcluded(t))
            //    .Select(t =>
            //    {
            //        var contracts = conventions.SelectViewModelContracts(t);

            //        return new
            //        {
            //            Contracts = conventions.SelectViewModelContracts(t),
            //            Implementation = t,
            //            Lifestyle = conventions.IsShellViewModel(contracts, t) ?
            //               Lifestyle.Singleton :
            //               Lifestyle.Transient
            //        };
            //    })
            //    .ForEach(descriptor =>
            //    {
            //        var builder = EntryBuilder.For(descriptor.Contracts.First())
            //            .WithLifestyle(descriptor.Lifestyle);

            //        foreach (var c in descriptor.Contracts.Skip(1))
            //        {
            //            builder = builder.Forward(c);
            //        }

            //        container.Register(builder);
            //    });

            //allTypes.Where(t => conventions.IsView(t) && !conventions.IsExcluded(t))
            //    .Select(t =>
            //    {
            //        var contracts = conventions.SelectViewContracts(t);

            //        return new
            //        {
            //            Contracts = contracts,
            //            Implementation = t,
            //            Lifestyle = conventions.IsShellView(contracts, t) ?
            //             Lifestyle.Singleton :
            //             Lifestyle.Transient
            //        };
            //    })
            //    .ForEach(descriptor =>
            //    {
            //        var builder = EntryBuilder.For(descriptor.Contracts.First())
            //            .WithLifestyle(descriptor.Lifestyle);

            //        foreach (var c in descriptor.Contracts.Skip(1))
            //        {
            //            builder = builder.Forward(c);
            //        }

            //        container.Register(builder);
            //    });
        }
    }
}
