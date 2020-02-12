
namespace Radical.Windows.ComponentModel
{
    /// <summary>
    /// Defines that a ViewModel expects life-cycle notifications from the view.
    /// </summary>
    public interface IExpectViewLoadedCallback
    {
        /// <summary>
        /// Called when the view is loaded.
        /// </summary>
        void OnViewLoaded();
    }
}
