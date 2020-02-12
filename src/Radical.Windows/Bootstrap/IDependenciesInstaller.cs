using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;

namespace Radical.Windows.Bootstrap
{
    public interface IDependenciesInstaller
    {
        void Install(BootstrapConventions conventions, IServiceCollection services, IEnumerable<Type> assemblyScanningResults);
    }
}
