using FakeItEasy;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Radical.ComponentModel.Messaging;
using Radical.Windows.Presentation.Boot;
using Radical.Windows.ComponentModel;
using Radical.Windows.Presentation.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace Radical.Windows.Tests.Conventions
{
    [TestClass]
    public class GenerateViewModelStaticResourceKeyTests
    {
        [TestMethod, TestCategory("Conventions")]
        public void GenerateViewModelStaticResourceKey_should_generate_expected_key()
        {
            var obj = new GenericParameterHelper();
            var expected = obj.GetType().Name;
            var conventions = new ConventionsHandler(A.Fake<IMessageBroker>(), A.Fake<IReleaseComponents>(), new BootstrapConventions());
            var key = conventions.GenerateViewModelStaticResourceKey(obj);

            Assert.AreEqual(expected, key);
        }

        [TestMethod, TestCategory("Conventions")]
        public void DefaultGenerateViewModelStaticResourceKey_should_generate_expected_key() 
        {
            var obj = new GenericParameterHelper();
            var expected = obj.GetType().Name;
            var conventions = new ConventionsHandler(A.Fake<IMessageBroker>(), A.Fake<IReleaseComponents>(), new BootstrapConventions());
            var key = conventions.DefaultGenerateViewModelStaticResourceKey(obj);

            Assert.AreEqual(expected, key);
        }
    }
}
