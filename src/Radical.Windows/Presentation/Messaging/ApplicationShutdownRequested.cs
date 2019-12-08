using System;
using Radical.Messaging;

namespace Radical.Windows.Presentation.Messaging
{
    /// <summary>
    /// Notifies that a request to shutdown request has been issued.
    /// </summary>
    public class ApplicationShutdownRequested
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationShutdownRequested"/> class.
        /// </summary>
        /// <param name="reason">The reason.</param>
        public ApplicationShutdownRequested( Boot.ApplicationShutdownReason reason )
        {
            this.Reason = reason;
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="ApplicationShutdownRequested"/> is cancelled.
        /// </summary>
        /// <value>
        ///   <c>true</c> if cancel; otherwise, <c>false</c>.
        /// </value>
        public bool Cancel { get; set; }

        /// <summary>
        /// Gets the shutdown reason.
        /// </summary>
        /// <value>
        /// The shutdown reason.
        /// </value>
        public Boot.ApplicationShutdownReason Reason { get; private set; }
    }
}
