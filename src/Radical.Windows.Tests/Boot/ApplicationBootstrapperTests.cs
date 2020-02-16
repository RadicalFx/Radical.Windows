using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Radical.Windows.Bootstrap;
using Radical.Windows.ComponentModel;
using Radical.Windows.Hosting;
using Radical.Windows.Tests.Boot.Presentation;
using System;
using System.Threading;
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
        public async Task Application_can_boot_using_host_builder()
        {
            var host = new HostBuilder()
                .AddRadicalApplication(configuration =>
                {
                    configuration.UseAsShell<MainView>();
                })
                .Build();

            await host.StartAsync();

            var resolvedViaHost = host.Services.GetService<IViewResolver>();

            Assert.IsNotNull(resolvedViaHost);

            using (host)
            {
                await host?.StopAsync();
            }
        }
    }
}