using Microsoft.VisualStudio.TestTools.UnitTesting;
using Radical.Windows.ComponentModel;
using System;

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
    }
}
