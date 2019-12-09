namespace Radical.Windows.Presentation.Messaging
{
    /// <summary>
    /// Domain event that identifies that a view model has been shown.
    /// </summary>
    public class ViewModelShown
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ViewModelShown"/> class.
        /// </summary>
        /// <param name="viewModel">The view model.</param>
        public ViewModelShown( object viewModel )
        {
            ViewModel = viewModel;
        }

        /// <summary>
        /// Gets the view model.
        /// </summary>
        public object ViewModel { get; private set; }
    }
}
