namespace Radical.Windows.Messaging
{
    /// <summary>
    /// Domain event that identifies that a view model has been activated.
    /// </summary>
    public class ViewModelActivated
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ViewModelActivated"/> class.
        /// </summary>
        /// <param name="viewModel">The view model.</param>
        public ViewModelActivated(object viewModel)
        {
            ViewModel = viewModel;
        }

        /// <summary>
        /// Gets the view model.
        /// </summary>
        public object ViewModel { get; private set; }
    }
}
