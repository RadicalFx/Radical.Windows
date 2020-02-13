using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Radical.Windows.Tests
{
    [TestClass()]
    public sealed class TestsSuiteInitialize
    {
        [AssemblyInitialize()]
        public static void AssemblyInit(TestContext context)
        {
            new System.Windows.Application();
        }
    }
}
