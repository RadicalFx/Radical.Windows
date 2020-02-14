using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading;

namespace Radical.Windows.Tests
{
    public class SingleThreadedApartmentTestMethodAttribute : TestMethodAttribute
    {
        public override TestResult[] Execute(ITestMethod testMethod)
        {
            if (Thread.CurrentThread.GetApartmentState() == ApartmentState.STA)
                return base.Execute(testMethod);

            TestResult[] result = null;
            var thread = new Thread(() => result = base.Execute(testMethod));
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
            thread.Join();
            return result;
        }
    }
}
