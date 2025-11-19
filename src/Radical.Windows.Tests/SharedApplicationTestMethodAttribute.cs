using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace Radical.Windows.Tests
{
    public class SharedApplicationTestMethodAttribute : TestMethodAttribute
    {
        class TestExecutionContext
        {
            public ITestMethod TestMethod { get; set; }
            public TestResult[] Results { get; private set; }
            public Func<Task<TestResult[]>> ExecuteAsync { get; internal set; }

            public async Task ExecuteTestAsync() 
            {
                Results = await ExecuteAsync();
            }
        }

        static readonly Thread worker = null;
        static readonly object syncLock = new object();
        static TestExecutionContext todo = null;
        static ManualResetEvent waitHandle = new ManualResetEvent(false);

        static SharedApplicationTestMethodAttribute()
        {
            worker = new Thread(()=> 
            {
                _ = new Application();
                while (true) 
                {
                    lock (syncLock)
                    {
                        if (todo == null)
                        {
                            continue;
                        }

                        todo.ExecuteTestAsync().GetAwaiter().GetResult();
                        todo = null;
                        waitHandle.Set();
                    }
                }
            });
            worker.IsBackground = true;
            worker.SetApartmentState(ApartmentState.STA);
            worker.Start();
        }

        public override Task<TestResult[]> ExecuteAsync(ITestMethod testMethod)
        {
            var context = new TestExecutionContext()
            {
                TestMethod = testMethod,
                ExecuteAsync = () => base.ExecuteAsync(testMethod)
            };

            todo = context;
            WaitHandle.WaitAll(new []{ waitHandle });
            waitHandle.Reset();

            return Task.FromResult(context.Results);
        }
    }
}
