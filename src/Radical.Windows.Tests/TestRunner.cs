using System;
using System.Threading;

namespace Radical.Windows.Tests
{
    class TestRunner
    {
        readonly Action test;
        readonly ApartmentState state;
        Exception ex;

        private TestRunner(ApartmentState state, Action test)
        {
            this.state = state;
            this.test = test;
        }

        public static void Execute(ApartmentState state, Action test)
        {
            var runner = new TestRunner(state, test);
            runner.Execute();
        }

        private void Execute()
        {
            var worker = new Thread(() =>
            {
                try
                {
                    this.test();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    this.ex = e;
                }
            });

            worker.SetApartmentState(this.state);
            worker.Start();
            worker.Join();

            if (this.ex != null)
            {
                throw this.ex;
            }
        }
    }
}
