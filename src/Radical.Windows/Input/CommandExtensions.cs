using Radical.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

namespace Radical.Windows.Input
{
    public static class CommandExtensions
    {
        public static IEnumerable<InputGesture> GetGestures(this ICommand source)
        {
            Ensure.That(source).Named("source").IsNotNull();

            IEnumerable<InputGesture> gestures = null;

            if (source is DelegateCommand)
            {
                var cmd = (DelegateCommand)source;
                if (cmd.InputBindings != null && cmd.InputBindings.Count > 0)
                {
                    gestures = cmd.InputBindings.OfType<InputBinding>().Select(ib => ib.Gesture);
                }
            }
            else if (source is RoutedCommand cmd)
            {
                if (cmd.InputGestures != null && cmd.InputGestures.Count > 0)
                {
                    gestures = cmd.InputGestures.OfType<InputGesture>();
                }
            }
            else
            {
                throw new NotSupportedException(string.Format("Unsupported command type: {0}", source));
            }

            return gestures ?? new InputGesture[0];
        }
    }
}
