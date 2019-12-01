using Microsoft.Xaml.Behaviors;
using System.Windows.Controls;

namespace Radical.Windows.Behaviors
{
    public class DisableManipulationBoundaryFeedback : Behavior<ListBox>
    {
        protected override void OnAttached()
        {
            base.OnAttached();
            this.AssociatedObject.ManipulationBoundaryFeedback += listBox_ManipulationBoundaryFeedback;
        }

        void listBox_ManipulationBoundaryFeedback(object sender, System.Windows.Input.ManipulationBoundaryFeedbackEventArgs e)
        {
            e.Handled = true;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            this.AssociatedObject.ManipulationBoundaryFeedback -= listBox_ManipulationBoundaryFeedback;
        }
    }
}
