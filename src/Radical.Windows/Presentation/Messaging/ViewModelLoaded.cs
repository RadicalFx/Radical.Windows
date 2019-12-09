namespace Radical.Windows.Presentation.Messaging
{
    /// <summary>
    /// Domain event that identifies that a view model has been loaded.
    /// </summary>
    public class ViewModelLoaded
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ViewModelLoaded"/> class.
        /// </summary>
        /// <param name="viewModel">The view model.</param>
        public ViewModelLoaded(object viewModel)
        {
            ViewModel = viewModel;
        }

        /// <summary>
        /// Gets the view model.
        /// </summary>
        public object ViewModel { get; private set; }
    }
}
