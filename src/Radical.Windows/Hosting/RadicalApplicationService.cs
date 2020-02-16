using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace Radical.Windows.Hosting
{
    class RadicalApplicationService : IHostedService
    {
        IServiceProvider serviceProvider;
        BootstrapConfiguration bootstrapConfiguration;
        RadicalApplication radicalApplication;

        public RadicalApplicationService(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
            bootstrapConfiguration = serviceProvider.GetRequiredService<BootstrapConfiguration>();
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            var application = serviceProvider.GetRequiredService<Application>();
            radicalApplication = RadicalApplication.ExternallyManagedBy(application, serviceProvider, bootstrapConfiguration);
            radicalApplication.Boot();

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            radicalApplication.Shutdown();
            return Task.CompletedTask;
        }
    }
}
