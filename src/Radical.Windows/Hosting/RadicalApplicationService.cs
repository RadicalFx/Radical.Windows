using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Radical.Windows.Bootstrap;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Radical.Windows.Hosting
{
    class RadicalApplicationService : IHostedService
    {
        IServiceProvider serviceProvider;
        BootstrapConfiguration bootstrapConfiguration;

        public RadicalApplicationService(IServiceProvider serviceProvider, BootstrapConfiguration bootstrapConfiguration)
        {
            this.serviceProvider = serviceProvider;
            this.bootstrapConfiguration = bootstrapConfiguration;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            //boot
            var features = serviceProvider.GetServices<IFeature>();
            foreach (var feature in features)
            {
                feature.Setup(serviceProvider, bootstrapConfiguration);
            }

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
