using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading;
using System.Threading.Tasks;

namespace Radical.Windows.Tests
{
    public class SingleThreadedApartmentTestMethodAttribute : TestMethodAttribute
    {
        public override Task<TestResult[]> ExecuteAsync(ITestMethod testMethod)
        {
            if (Thread.CurrentThread.GetApartmentState() == ApartmentState.STA)
                return base.ExecuteAsync(testMethod);

            TestResult[] result = null;
            var thread = new Thread(() => result = base.ExecuteAsync(testMethod).GetAwaiter().GetResult());
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
            thread.Join();
            return Task.FromResult(result);
        }
    }
}
