using Microsoft.Extensions.Hosting;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Radical.Windows.Bootstrap;
using Radical.Windows.ComponentModel;
using Radical.Windows.Tests.Boot.Presentation;
using System;
using System.Threading.Tasks;

namespace Radical.Windows.Tests.Boot
{
    [TestClass]
    public class ApplicationBootstrapperTests
    {
        [SharedApplicationTestMethod]
        public void ApplicationBoot_process_should_create_valid_container()
        {
            IServiceProvider container = null;
            var bootstrapper = new ApplicationBootstrapper();
            bootstrapper.DisableAutoBoot();
            bootstrapper.OnBoot(serviceProvider => container = serviceProvider);
            bootstrapper.Boot();

            var viewResolver = container.GetService<IViewResolver>();

            Assert.IsNotNull(viewResolver);
        }

        [SharedApplicationTestMethod]
        public async Task ApplicationBoot_should_used_host_builder()
        {
            var configuration = new BootstrapConfiguration();
            configuration.UseShell<MainView>();

            var host = new HostBuilder()
                .AddRadicalApplication(configuration)
                .Build();

            await host.StartAsync();
            var container = ApplicationBootstrapper.Boot(configuration, host.Services);

            var resolvedByHost = host.Services.GetService<IViewResolver>();
            var resolvedByAppplicationBootstrapper = container.GetService<IViewResolver>();

            Assert.IsNotNull(resolvedByHost);
            Assert.IsNotNull(resolvedByAppplicationBootstrapper);

            using (host)
            {
                await host?.StopAsync();
            }
        }
    }

    static class Ext
    {
        public static IHostBuilder AddRadicalApplication(this IHostBuilder hostBuilder, BootstrapConfiguration bootstrapConfiguration)
        {
            hostBuilder.ConfigureServices(serviceCollection =>
            {
                bootstrapConfiguration.UseServiceCollection(serviceCollection);
            });

            return hostBuilder;
        }
    }
}