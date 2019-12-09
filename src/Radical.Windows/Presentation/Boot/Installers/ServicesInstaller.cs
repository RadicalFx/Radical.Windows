using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Radical.Windows.Presentation.Boot.Installers
{
    class ServicesInstaller : IDependenciesInstaller
    {
        public void Install(BootstrapConventions conventions, IServiceCollection services, IEnumerable<Type> assemblyScanningResults)
        {
            //assemblyScanningResults.Where(t => conventions.IsService(t) && !conventions.IsExcluded(t))
            //    .Select(t => new
            //    {
            //        Contracts = conventions.SelectServiceContracts(t),
            //        Implementation = t
            //    })
            //    .ForEach(descriptor =>
            //    {
            //        var entry = EntryBuilder.For(descriptor.Contracts.First())
            //                .ImplementedBy(descriptor.Implementation)
            //                .Overridable();

            //        foreach (var fw in descriptor.Contracts.Skip(1))
            //        {
            //            entry = entry.Forward(fw);
            //        }

            //        container.Register(entry);
            //    });
        }
    }
}
