using Radical.Windows.Presentation.ComponentModel;

namespace Radical.Windows.Presentation.Boot
{
    /// <summary>
    /// Allows a third party component to be notified when the
    /// application lifecycle state changes.
    /// </summary>
    [ToolkitComponentAttribute]
    public interface IExpectBootCallback
    {
        /// <summary>
        /// Called when application boot process is completed.
        /// </summary>
        void OnBootCompleted();
    }
}
