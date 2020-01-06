using Radical.Validation;
using System;
using System.Windows.Threading;

namespace Radical.Windows.Presentation
{
    /// <summary>
    /// Allows to execute and action every time the given amount of time elapses.
    /// </summary>
    public class Repeat
    {
        /// <summary>
        /// A repeater for the repeat infrastucture.
        /// </summary>
        public class Repeater
        {
            DispatcherTimer timer;

            internal Repeater( TimeSpan delay )
            {
                timer = new DispatcherTimer()
                {
                    Interval = delay
                };

                timer.Tick += ( s, a ) =>
                {
                    timer.Stop();
                    action();
                    timer.Start();
                };
            }

            Action action;

            /// <summary>
            /// After the delay executes the given action.
            /// </summary>
            /// <param name="action">The action.</param>
            public void This( Action action )
            {
                Ensure.That( action ).Named( () => action ).IsNotNull();

                this.action = action;
                timer.Start();
            }

            /// <summary>
            /// Stops this repeater.
            /// </summary>
            public void Stop() 
            {
                timer.Stop();
            }
        }

        /// <summary>
        /// Waits for the specified delay.
        /// </summary>
        /// <param name="delay">The delay.</param>
        /// <returns>A waiter ready to be configured.</returns>
        public static Repeater Every( TimeSpan delay )
        {
            return new Repeater( delay );
        }
    }
}
