using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Radical.Windows.Tests
{
    [TestClass()]
    public class ResourcesTests
    {
        [TestMethod]
        public void GrayscaleEffect_ctor_should_load_resource() 
        {
            new Effects.GrayscaleEffect();
        }
    }
}
