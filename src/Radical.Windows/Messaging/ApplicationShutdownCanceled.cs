using Radical.Windows.Bootstrap;

namespace Radical.Windows.Messaging
{
    /// <summary>
    /// Notifies that the application shutdown has been canceled.
    /// </summary>
    public class ApplicationShutdownCanceled
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationShutdownCanceled"/> class.
        /// </summary>
        /// <param name="reason">The reason.</param>
        public ApplicationShutdownCanceled( ApplicationShutdownReason reason )
        {
            Reason = reason;
        }

        /// <summary>
        /// Gets the shutdown reason.
        /// </summary>
        /// <value>
        /// The shutdown reason.
        /// </value>
        public ApplicationShutdownReason Reason { get; private set; }
    }
}
