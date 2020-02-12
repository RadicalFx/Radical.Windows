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
            if (!UriParser.IsKnownScheme("pack"))
                new System.Windows.Application();

            new Effects.GrayscaleEffect();
        }
    }
}
