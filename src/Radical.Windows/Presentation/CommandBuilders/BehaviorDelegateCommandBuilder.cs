using Radical.Windows.Behaviors;
using System.Windows;

namespace Radical.Windows.CommandBuilders
{
    class BehaviorDelegateCommandBuilder : DelegateCommandBuilder
    {
        public override bool CanCreateCommand(System.Windows.PropertyPath path, System.Windows.DependencyObject target)
        {
            return path != null && target is INotifyAttachedOjectLoaded;
        }

        public override object GetDataContext(System.Windows.DependencyObject target)
        {
            return ((INotifyAttachedOjectLoaded)target)
                    .GetAttachedObject<FrameworkElement>()
                    .DataContext;
        }
    }
}
