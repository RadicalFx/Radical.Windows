using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;

namespace Radical.Windows.Bootstrap
{
    /// <summary>
    /// Installs dependencies in the service collection at boot time. Types implementing this
    /// interface found by the assembly scanning are automatically invoked.
    /// </summary>
    public interface IDependenciesInstaller
    {
        /// <summary>
        /// Installs dependencies in the given service collection.
        /// </summary>
        /// <param name="conventions">The bootstrap conventions.</param>
        /// <param name="services">The service collection.</param>
        /// <param name="assemblyScanningResults">The types found by the assembly scanning.</param>
        void Install(BootstrapConventions conventions, IServiceCollection services, IEnumerable<Type> assemblyScanningResults);
    }
}
