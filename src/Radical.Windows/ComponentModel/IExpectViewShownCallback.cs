
namespace Radical.Windows.ComponentModel
{
    /// <summary>
    /// Defines that a ViewModel expects life-cycle notifications from the view.
    /// </summary>
    public interface IExpectViewShownCallback
    {
        /// <summary>
        /// Called when the view is shown.
        /// </summary>
        void OnViewShown();
    }
}