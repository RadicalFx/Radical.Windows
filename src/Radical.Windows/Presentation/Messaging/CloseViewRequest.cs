using System;
using Radical.Messaging;

namespace Radical.Windows.Presentation.Messaging
{
    /// <summary>
    /// Issues a request to close the view currently associated with the view model that sends the message.
    /// </summary>
    public class CloseViewRequest
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CloseViewRequest"/> class.
        /// </summary>
        /// <param name="viewOwner">The view owner.</param>
        public CloseViewRequest( object viewOwner )
        {
            ViewOwner = viewOwner;
            DialogResult = null;
        }

        /// <summary>
        /// Gets the view owner.
        /// </summary>
        public object ViewOwner { get; private set; }

        /// <summary>
        /// Gets or sets the dialog result.
        /// </summary>
        /// <value>
        /// The dialog result.
        /// </value>
        public bool? DialogResult { get; set; }
    }
}
