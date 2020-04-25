namespace Radical.Windows.Bootstrap
{
    /// <summary>
    /// The application shutdown arguments.
    /// </summary>
    public class ApplicationShuttingDownArgs
    {
        /// <summary>
        /// Gets a value indicating whether this application boot is completed.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is boot completed; otherwise, <c>false</c>.
        /// </value>
        public bool IsBootCompleted { get; internal set; }

        /// <summary>
        /// Gets the application shutdown reason.
        /// </summary>
        /// <value>
        /// The application shutdown reason.
        /// </value>
        public ApplicationShutdownReason Reason { get; internal set; }
    }
}
