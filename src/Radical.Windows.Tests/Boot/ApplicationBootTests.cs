using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Radical.Windows.ComponentModel;
using Radical.Windows.Hosting;
using System;
using System.Threading.Tasks;
using System.Windows;
using Unity.Microsoft.DependencyInjection;

namespace Radical.Windows.Tests.Boot
{
    [TestClass]
    public class ApplicationBootTests
    {
        [SharedApplicationTestMethod]
        public void ApplicationBoot_process_should_create_valid_container()
        {
            var bootCompleted = false;
            IServiceProvider container = null;

            var radicalApplication = Application.Current.AddRadicalApplication(configuration => 
            {
                configuration.DisableAutoBoot();
                configuration.OnServiceProviderCreated(serviceProvider => container = serviceProvider);
                configuration.OnBootCompleted(_ => bootCompleted = true);
            });

            radicalApplication.Boot();

            var viewResolver = container.GetService<IViewResolver>();

            Assert.IsNotNull(viewResolver);
            Assert.IsTrue(bootCompleted);
        }

        [SharedApplicationTestMethod]
        public async Task Application_can_boot_using_host_builder()
        {
            var bootCompleted = false;

            var host = new HostBuilder()
                .AddRadicalApplication(configuration =>
                {
                    configuration.OnBootCompleted(_ => bootCompleted = true);
                })
                .Build();

            await host.StartAsync();

            var viewResolver = host.Services.GetService<IViewResolver>();

            Assert.IsNotNull(viewResolver);
            Assert.IsTrue(bootCompleted);

            using (host)
            {
                await host?.StopAsync();
            }
        }

        [SharedApplicationTestMethod]
        public async Task Application_using_host_builder_can_resolve_configured_services()
        {
            var host = new HostBuilder()
                .ConfigureServices(serviceCollection =>
                {
                    serviceCollection.AddSingleton<SampleDependency>();
                })
                .AddRadicalApplication(_ => { })
                .Build();

            await host.StartAsync();

            var customDependency = host.Services.GetService<SampleDependency>();
            var viewResolver = host.Services.GetService<IViewResolver>();

            Assert.IsNotNull(customDependency);
            Assert.IsNotNull(viewResolver);

            using (host)
            {
                await host?.StopAsync();
            }
        }

        [SharedApplicationTestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Application_using_host_builder_can_call_AddRadicalApplication_only_once()
        {
            var host = new HostBuilder()
                .AddRadicalApplication(_ => { })
                .AddRadicalApplication(_ => { })
                .Build();
        }

        [SharedApplicationTestMethod]
        public async Task Application_using_host_builder_can_call_configure_services_in_any_order()
        {
            var host = new HostBuilder()
                .AddRadicalApplication(_ => { })
                .ConfigureServices(serviceCollection =>
                {
                    serviceCollection.AddSingleton<SampleDependency>();
                })
                .Build();

            await host.StartAsync();

            var customDependency = host.Services.GetService<SampleDependency>();
            var viewResolver = host.Services.GetService<IViewResolver>();

            Assert.IsNotNull(customDependency);
            Assert.IsNotNull(viewResolver);

            using (host)
            {
                await host?.StopAsync();
            }
        }

        [SharedApplicationTestMethod]
        public async Task Application_can_boot_using_host_builder_and_Autofac()
        {
            var host = new HostBuilder()
                .UseServiceProviderFactory(new AutofacServiceProviderFactory())
                .AddRadicalApplication(_ => { })
                .Build();

            await host.StartAsync();

            var resolvedViaHost = host.Services.GetService<IViewResolver>();

            Assert.AreEqual(host.Services.GetType(), typeof(AutofacServiceProvider));
            Assert.IsNotNull(resolvedViaHost);

            using (host)
            {
                await host?.StopAsync();
            }
        }

        [SharedApplicationTestMethod]
        public async Task Application_can_boot_using_host_builder_and_Unity()
        {
            var host = new HostBuilder()
                .UseUnityServiceProvider()
                .AddRadicalApplication(_ => { })
                .Build();

            await host.StartAsync();

            var resolvedViaHost = host.Services.GetService<IViewResolver>();

            Assert.AreEqual(host.Services.GetType(), typeof(Unity.Microsoft.DependencyInjection.ServiceProvider));
            Assert.IsNotNull(resolvedViaHost);

            using (host)
            {
                await host?.StopAsync();
            }
        }
    }

    class SampleDependency { }
}