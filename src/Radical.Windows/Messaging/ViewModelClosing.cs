using System;

namespace Radical.Windows.Messaging
{
    /// <summary>
    /// Domain event that identifies that a view model is closing.
    /// </summary>
    [Obsolete("ViewModelClosing has been obsoleted and will be removed in v5.0.0. The infrastructure never publishes it: to cancel closing a view, implement IExpectViewClosingCallback on the view model instead.", true)]
    public class ViewModelClosing
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ViewModelClosing"/> class.
        /// </summary>
        /// <param name="viewModel">The view model.</param>
        public ViewModelClosing(object viewModel)
        {
            ViewModel = viewModel;
        }

        /// <summary>
        /// Gets the view model.
        /// </summary>
        public object ViewModel { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether the <see cref="ViewModelClosing"/> should be canceled.
        /// </summary>
        /// <value>
        ///   <c>true</c> if cancel; otherwise, <c>false</c>.
        /// </value>
        public bool Cancel { get; set; }
    }
}
