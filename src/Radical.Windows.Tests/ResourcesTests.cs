using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Radical.Windows.Tests
{
    [TestClass()]
    public class ResourcesTests
    {
        [SharedApplicationTestMethod]
        public void GrayscaleEffect_ctor_should_load_resource() 
        {
            new Effects.GrayscaleEffect();
        }
    }
}
