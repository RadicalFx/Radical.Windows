using Microsoft.Xaml.Behaviors;
using System.Windows.Controls;

namespace Radical.Windows.Behaviors
{
    /// <summary>
    /// A behavior that disables the manipulation boundary feedback, the window bounce
    /// on touch devices, when panning a <see cref="ListBox"/> past its boundaries.
    /// </summary>
    public class DisableManipulationBoundaryFeedback : Behavior<ListBox>
    {
        /// <inheritdoc/>
        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.ManipulationBoundaryFeedback += listBox_ManipulationBoundaryFeedback;
        }

        void listBox_ManipulationBoundaryFeedback(object sender, System.Windows.Input.ManipulationBoundaryFeedbackEventArgs e)
        {
            e.Handled = true;
        }

        /// <inheritdoc/>
        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.ManipulationBoundaryFeedback -= listBox_ManipulationBoundaryFeedback;
        }
    }
}
