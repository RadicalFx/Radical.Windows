using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Radical.Windows.Bootstrap;
using System;

namespace Radical.Windows.Hosting
{
    public static class HostBuilderExtensions
    {
        public static IHostBuilder AddRadicalApplication(this IHostBuilder hostBuilder, Action<BootstrapConfiguration> configure)
        {
            var configuration = new BootstrapConfiguration();
            configure(configuration);

            hostBuilder.ConfigureServices((context,serviceCollection) =>
            {
                configuration.PopulateServiceCollection(serviceCollection);

                serviceCollection.AddHostedService<RadicalApplicationService>();
            });

            return hostBuilder;
        }
    }
}
