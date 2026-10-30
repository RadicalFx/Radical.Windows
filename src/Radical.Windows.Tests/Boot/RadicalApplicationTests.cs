using Microsoft.VisualStudio.TestTools.UnitTesting;
using Radical.Windows.ComponentModel;
using System;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;

namespace Radical.Windows.Tests.Boot
{
    [TestClass]
    public class RadicalApplicationTests
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
        public void Application_without_a_shell_boots_without_showing_windows()
        {
            var bootCompleted = false;
            var mainWindowBeforeBoot = Application.Current.MainWindow;
            var windowsBeforeBoot = Application.Current.Windows.Count;

            var radicalApplication = Application.Current.AddRadicalApplication(configuration =>
            {
                configuration.DisableAutoBoot();
                configuration.OnBootCompleted(_ => bootCompleted = true);
            });

            radicalApplication.Boot();

            Assert.IsTrue(bootCompleted);
            Assert.AreSame(mainWindowBeforeBoot, Application.Current.MainWindow);
            Assert.AreEqual(windowsBeforeBoot, Application.Current.Windows.Count);
        }

        [SharedApplicationTestMethod]
        public void ApplicationBoot_process_should_allow_services_customization()
        {
            var bootCompleted = false;
            IServiceProvider container = null;

            var radicalApplication = Application.Current.AddRadicalApplication(configuration =>
            {
                configuration.ConfigureServices(services =>
                {
                    services.AddTransient<SampleDependency>();
                });
                configuration.DisableAutoBoot();
                configuration.OnServiceProviderCreated(serviceProvider => container = serviceProvider);
                configuration.OnBootCompleted(_ => bootCompleted = true);
            });

            radicalApplication.Boot();

            var service = container.GetService<SampleDependency>();

            Assert.IsNotNull(service);
            Assert.IsTrue(bootCompleted);
        }
    }
}
