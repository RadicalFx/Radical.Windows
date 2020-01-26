
namespace Radical.Windows.ComponentModel
{
    /// <summary>
    /// Defines that a ViewModel expects life-cycle notifications from the view.
    /// </summary>
    public interface IExpectViewActivatedCallback
    {
        /// <summary>
        /// Called when the view is activated.
        /// </summary>
        void OnViewActivated();
    }
}