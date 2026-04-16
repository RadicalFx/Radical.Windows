using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
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
            public ManualResetEventSlim Completed { get; } = new ManualResetEventSlim(false);

            public async Task ExecuteTestAsync() 
            {
                Results = await ExecuteAsync();
            }
        }

        static readonly Thread worker = null;
        static readonly object syncLock = new object();
        static readonly Queue<TestExecutionContext> todo = new Queue<TestExecutionContext>();
        static readonly SemaphoreSlim hasWork = new SemaphoreSlim(0);

        static SharedApplicationTestMethodAttribute()
        {
            worker = new Thread(()=> 
            {
                _ = new Application();
                while (true) 
                {
                    hasWork.Wait();

                    TestExecutionContext item = null;
                    lock (syncLock)
                    {
                        item = todo.Dequeue();
                    }

                    item.ExecuteTestAsync().GetAwaiter().GetResult();
                    item.Completed.Set();
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

            lock (syncLock)
            {
                todo.Enqueue(context);
            }

            hasWork.Release();
            try
            {
                context.Completed.Wait();
            }
            finally
            {
                context.Completed.Dispose();
            }

            return Task.FromResult(context.Results);
        }
    }
}
