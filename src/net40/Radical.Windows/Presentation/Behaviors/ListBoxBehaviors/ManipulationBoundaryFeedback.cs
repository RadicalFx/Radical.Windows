using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Interactivity;

namespace Topics.Radical.Windows.Presentation.Behaviors
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
