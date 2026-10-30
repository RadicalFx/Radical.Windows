using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Runtime.ExceptionServices;
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

            // Capture the execution context so that MSTest 4.2+ AsyncLocal values
            // (e.g. TestContext.Current) are propagated to the new STA thread.
            var capturedContext = ExecutionContext.Capture();

            TestResult[] result = null;
            ExceptionDispatchInfo capturedException = null;

            var thread = new Thread(() =>
            {
                if (capturedContext != null)
                {
                    ExecutionContext.Run(capturedContext, _ =>
                    {
                        try
                        {
                            result = base.ExecuteAsync(testMethod).GetAwaiter().GetResult();
                        }
                        catch (Exception ex)
                        {
                            capturedException = ExceptionDispatchInfo.Capture(ex);
                        }
                    }, null);
                }
                else
                {
                    result = base.ExecuteAsync(testMethod).GetAwaiter().GetResult();
                }
            });

            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
            thread.Join();

            capturedException?.Throw();
            return Task.FromResult(result);
        }
    }
}
