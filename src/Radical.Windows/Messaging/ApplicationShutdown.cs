using Radical.Windows.Presentation.Boot;

namespace Radical.Windows.Messaging
{
    /// <summary>
    /// Notifies that the application is shutting down.
    /// </summary>
    public class ApplicationShutdown
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationShutdown" /> class.
        /// </summary>
        /// <param name="reason">The reason.</param>
        public ApplicationShutdown( ApplicationShutdownReason reason)
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
