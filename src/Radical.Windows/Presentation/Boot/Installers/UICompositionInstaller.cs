using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;

namespace Radical.Windows.Presentation.Boot.Installers
{
    class UICompositionInstaller : IDependenciesInstaller
    {
        public void Install(BootstrapConventions conventions, IServiceCollection services, IEnumerable<Type> assemblyScanningResults)
        {
            //container.Register(
            //    EntryBuilder.For<IRegionManagerFactory>()
            //        .ImplementedBy<RegionManagerFactory>()
            //        .Overridable());

            //container.Register(
            //    EntryBuilder.For<IRegionService>()
            //        .ImplementedBy<RegionService>()
            //        .Overridable());

            //container.Register(
            //    EntryBuilder.For<IRegionManager>()
            //        .ImplementedBy<RegionManager>()
            //        .WithLifestyle(Lifestyle.Transient)
            //        .Overridable());
        }
    }
}
