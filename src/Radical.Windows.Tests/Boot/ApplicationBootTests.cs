using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Radical.Windows.ComponentModel;
using Radical.Windows.Hosting;
using Radical.Windows.Tests.Boot.Presentation;
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
            IServiceProvider container = null;

            var configuration = new BootstrapConfiguration();
            configuration.DisableAutoBoot();
            configuration.OnServiceProviderCreated(serviceProvider => container = serviceProvider);

            var bootstrapper = RadicalApplication.BindTo(Application.Current, configuration);
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

            var viewResolver = host.Services.GetService<IViewResolver>();

            Assert.IsNotNull(viewResolver);

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
                .AddRadicalApplication(configuration =>
                {
                    configuration.UseAsShell<MainView>();
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
        [ExpectedException(typeof(InvalidOperationException))]
        public void Application_using_host_builder_can_call_AddRadicalApplication_only_once()
        {
            var host = new HostBuilder()
                .AddRadicalApplication(configuration =>{})
                .AddRadicalApplication(configuration => { })
                .Build();
        }

        [SharedApplicationTestMethod]
        public async Task Application_using_host_builder_can_call_configure_services_in_any_order()
        {
            var host = new HostBuilder()
                .AddRadicalApplication(configuration =>
                {
                    configuration.UseAsShell<MainView>();
                })
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
                .AddRadicalApplication(configuration =>
                {
                    configuration.UseAsShell<MainView>();
                })
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
                .AddRadicalApplication(configuration =>
                {
                    configuration.UseAsShell<MainView>();
                })
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