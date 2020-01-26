
namespace Radical.Windows.ComponentModel
{
    /// <summary>
    /// Defines that a ViewModel expects life-cycle notifications from the view.
    /// </summary>
    public interface IExpectViewClosedCallback
    {
        /// <summary>
        /// Called when the view is closed.
        /// </summary>
        void OnViewClosed();
    }
}