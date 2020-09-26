using System.Windows;
using FakeItEasy;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Radical.ComponentModel.Messaging;
using Radical.Windows.Bootstrap;
using Radical.Windows.ComponentModel;
using Radical.Windows.Services;

namespace Radical.Windows.Tests.Conventions
{
    [TestClass]
    public class ShouldExposeViewModelAsStaticResourceTests
    {
        class ViewModelNoAttribute
        {
        }

        [ExposeViewModelAsStaticResource]
        class ViewModelWithAttribute
        {
        }

        class NopView : DependencyObject
        {
        }

        [TestMethod, TestCategory("Conventions")]
        public void ShouldExposeViewModelAsStaticResource_should_return_false_if_no_attribute()
        {
            var conventions = new ConventionsHandler(A.Fake<IMessageBroker>(), A.Fake<IReleaseComponents>(), new BootstrapConventions());
            var key = conventions.ShouldExposeViewModelAsStaticResource(new NopView(), new ViewModelNoAttribute());

            Assert.AreEqual(false, key);
        }

        [TestMethod, TestCategory("Conventions")]
        public void ShouldExposeViewModelAsStaticResource_should_return_false_if_attribute()
        {
            var conventions = new ConventionsHandler(A.Fake<IMessageBroker>(), A.Fake<IReleaseComponents>(), new BootstrapConventions());
            var key = conventions.ShouldExposeViewModelAsStaticResource(new NopView(), new ViewModelWithAttribute());

            Assert.AreEqual(true, key);
        }
    }
}
