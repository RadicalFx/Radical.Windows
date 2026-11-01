using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Radical.ComponentModel.Messaging;
using Radical.Windows.Messaging;
using System.Threading.Tasks;
using System.Windows;

namespace Radical.Windows.Tests.Boot
{
    [TestClass]
    public class RadicalApplicationShutdownTests
    {
        /*
         * Only canceled shutdowns are tested: a completed shutdown
         * would shut down the application shared by all the tests.
         */
        [SharedApplicationTestMethod]
        public async Task ShutdownAsync_should_be_canceled_by_an_async_ApplicationShutdownRequested_subscriber()
        {
            var shuttingDown = false;
            var subscriberCompleted = false;

            var radicalApplication = Application.Current.AddRadicalApplication(configuration =>
            {
                configuration.DisableAutoBoot();
                configuration.OnBootCompleted(serviceProvider =>
                {
                    var broker = serviceProvider.GetRequiredService<IMessageBroker>();
                    broker.Subscribe<ApplicationShutdownRequested>(serviceProvider, async (s, m) =>
                    {
                        await Task.Yield();
                        m.Cancel = true;
                        subscriberCompleted = true;
                    });
                });
                configuration.OnShuttingDown(_ => shuttingDown = true);
            });

            radicalApplication.Boot();

            await radicalApplication.ShutdownAsync();

            Assert.IsTrue(subscriberCompleted);
            Assert.IsFalse(shuttingDown);
        }
    }
}
