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
        static readonly AutoResetEvent hasWork = new AutoResetEvent(false);

        static SharedApplicationTestMethodAttribute()
        {
            worker = new Thread(()=> 
            {
                _ = new Application();
                while (true) 
                {
                    hasWork.WaitOne();

                    while (true)
                    {
                        TestExecutionContext item = null;
                        lock (syncLock)
                        {
                            if (todo.Count == 0)
                            {
                                break;
                            }

                            item = todo.Dequeue();
                        }

                        item.ExecuteTestAsync().GetAwaiter().GetResult();
                        item.Completed.Set();
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

            lock (syncLock)
            {
                todo.Enqueue(context);
            }

            hasWork.Set();
            context.Completed.Wait();

            return Task.FromResult(context.Results);
        }
    }
}
