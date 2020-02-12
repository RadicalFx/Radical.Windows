using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;

namespace Radical.Windows.Bootstrap.Installers
{
    static class ServiceCollectionExtensions
    {
        public static bool IsRegistered<ServiceType>(this IServiceCollection services) 
        {
            return services.IsRegistered(typeof(ServiceType));
        }

        public static bool IsRegistered(this IServiceCollection services, Type serviceType)
        {
            return services.Any(d => d.ServiceType == serviceType);
        }
    }
}
