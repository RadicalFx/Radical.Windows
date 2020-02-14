using FakeItEasy;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Radical.ComponentModel.Messaging;
using Radical.Windows.Bootstrap;
using Radical.Windows.ComponentModel;
using Radical.Windows.Services;
using Radical.Windows.Tests.Services.Presentation;
using System;

namespace Radical.Windows.Tests.Services
{
    [TestClass()]
    public class ViewResolverTests
    {
        [SingleThreadedApartmentTestMethod()]
        public void ViewResolver_resolve_should_resolve_expected_view() 
        {
            var conventions = new ConventionsHandler(A.Fake<IMessageBroker>(), A.Fake<IReleaseComponents>(), new BootstrapConventions());
            var container = A.Fake<IServiceProvider>();
            A.CallTo(() => container.GetService(typeof(AnEmptyView))).Returns(new AnEmptyView());
            var sut = new ViewResolver(container, conventions, new ResourcesRegistrationHolder());

            var view = sut.GetView<AnEmptyView>();

            Assert.IsNotNull(view);
        }
    }
}
