using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading;
using System.Windows;

namespace Radical.Windows.Tests
{
    public class SharedApplicationTestMethodAttribute : TestMethodAttribute
    {
        class TestExecutionContext
        {
            public ITestMethod TestMethod { get; set; }
            public TestResult[] Results { get; private set; }
            public Func<TestResult[]> Execute { get; internal set; }

            public void ExecuteTest() 
            {
                Results = Execute();
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
                new Application();
                while (true) 
                {
                    lock (syncLock) 
                    {
                        if (todo != null)
                        {
                            todo.ExecuteTest();
                            todo = null;
                            waitHandle.Set();
                        }
                        else 
                        {
                            continue;
                        }
                    }
                }
            });
            worker.IsBackground = true;
            worker.SetApartmentState(ApartmentState.STA);
            worker.Start();
        }

        public override TestResult[] Execute(ITestMethod testMethod)
        {
            var context = new TestExecutionContext()
            {
                TestMethod = testMethod,
                Execute = () => base.Execute(testMethod)
            };

            todo = context;
            WaitHandle.WaitAll(new []{ waitHandle });
            waitHandle.Reset();

            return context.Results;
        }
    }
}
