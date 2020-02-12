using System.ComponentModel;

namespace Radical.Windows.ComponentModel
{
    /// <summary>
    /// Defines that a ViewModel expects life-cycle notifications from the view.
    /// </summary>
    public interface IExpectViewClosingCallback
    {
        /// <summary>
        /// Called when the view is closing.
        /// </summary>
        /// <param name="e">The <see cref="System.ComponentModel.CancelEventArgs"/> instance containing the event data.</param>
        void OnViewClosing( CancelEventArgs e );
    }
}