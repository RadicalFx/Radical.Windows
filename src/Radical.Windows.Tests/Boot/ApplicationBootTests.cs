using Microsoft.VisualStudio.TestTools.UnitTesting;
using Radical.Windows.ComponentModel;
using System;
using System.Windows;

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
    }
}