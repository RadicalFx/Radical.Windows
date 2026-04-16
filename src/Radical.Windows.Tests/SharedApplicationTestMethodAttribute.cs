using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System;
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace Radical.Windows.Tests
{
    public class SharedApplicationTestMethodAttribute : TestMethodAttribute
    {
        class WorkItem
        {
            public Func<Task<TestResult[]>> ExecuteAsync { get; set; }
            // Execution context captured from the MSTest runner thread so that
            // AsyncLocal values (including TestContext.Current set by MSTest 4.2+)
            // are visible when the work runs on the shared STA thread.
            public ExecutionContext CapturedContext { get; set; }
            public TestResult[] Results { get; set; }
            public ExceptionDispatchInfo Exception { get; set; }
            public ManualResetEventSlim Completed { get; } = new ManualResetEventSlim(false);
        }

        static readonly Thread worker = null;
        static readonly object syncLock = new object();
        static readonly Queue<WorkItem> pending = new Queue<WorkItem>();
        static readonly SemaphoreSlim hasWork = new SemaphoreSlim(0);

        static SharedApplicationTestMethodAttribute()
        {
            worker = new Thread(()=> 
            {
                _ = new Application();
                while (true) 
                {
                    hasWork.Wait();

                    WorkItem item;
                    lock (syncLock)
                    {
                        item = pending.Dequeue();
                    }

                    try
                    {
                        if (item.CapturedContext != null)
                        {
                            // Run the test body within the captured execution context so that
                            // MSTest 4.2+ AsyncLocal values (e.g. TestContext.Current) are visible.
                            ExceptionDispatchInfo capturedException = null;
                            TestResult[] capturedResults = null;
                            ExecutionContext.Run(item.CapturedContext, _ =>
                            {
                                try
                                {
                                    capturedResults = item.ExecuteAsync().GetAwaiter().GetResult();
                                }
                                catch (Exception ex)
                                {
                                    capturedException = ExceptionDispatchInfo.Capture(ex);
                                }
                            }, null);
                            item.Results = capturedResults;
                            item.Exception = capturedException;
                        }
                        else
                        {
                            item.Results = item.ExecuteAsync().GetAwaiter().GetResult();
                        }
                    }
                    catch (Exception ex)
                    {
                        item.Exception = ExceptionDispatchInfo.Capture(ex);
                    }
                    finally
                    {
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
            // Capture the current execution context. MSTest 4.2+ stores TestContext.Current
            // in an AsyncLocal which is only visible within the current execution context.
            // We must propagate it to the shared STA worker thread.
            var capturedContext = ExecutionContext.Capture();

            var item = new WorkItem
            {
                ExecuteAsync = () => base.ExecuteAsync(testMethod),
                CapturedContext = capturedContext
            };

            lock (syncLock)
            {
                pending.Enqueue(item);
            }

            hasWork.Release();
            try
            {
                item.Completed.Wait();
            }
            finally
            {
                item.Completed.Dispose();
            }

            item.Exception?.Throw();
            return Task.FromResult(item.Results);
        }
    }
}
