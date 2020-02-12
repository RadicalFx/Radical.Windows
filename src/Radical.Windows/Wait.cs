using Radical.Validation;
using System;
using System.Windows.Threading;

namespace Radical.Windows
{
    /// <summary>
    /// Allows to execute and action after a user defined amount of time.
    /// </summary>
    public class Wait
    {
        /// <summary>
        /// A waiter for the Wait infrastucture.
        /// </summary>
        public class Waiter
        {
            readonly DispatcherTimer timer;

            internal Waiter( TimeSpan delay )
            {
                timer = new DispatcherTimer()
                {
                    Interval = delay
                };

                timer.Tick += ( s, a ) =>
                {
                    timer.Stop();
                    action();
                };
            }

            Action action;

            /// <summary>
            /// After the delay executes the given action.
            /// </summary>
            /// <param name="action">The action.</param>
            public void AndThen( Action action )
            {
                Ensure.That( action ).Named( () => action ).IsNotNull();

                this.action = action;
                timer.Start();
            }
        }

        /// <summary>
        /// Waits for the specified delay.
        /// </summary>
        /// <param name="delay">The delay.</param>
        /// <returns>A waiter ready to be configured.</returns>
        public static Waiter For( TimeSpan delay )
        {
            return new Waiter( delay );
        }
    }
}
