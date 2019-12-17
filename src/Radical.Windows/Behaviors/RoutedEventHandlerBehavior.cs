using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace Radical.Windows.Behaviors
{
    public class Handle : RadicalBehavior<FrameworkElement>
    {
        #region Dependency Property: RoutedEvent

        public static readonly DependencyProperty RoutedEventProperty = DependencyProperty.Register(
            "RoutedEvent",
            typeof(RoutedEvent),
            typeof(Handle),
            new PropertyMetadata(null));

        public RoutedEvent RoutedEvent
        {
            get { return (RoutedEvent)GetValue(RoutedEventProperty); }
            set { SetValue(RoutedEventProperty, value); }
        }

        #endregion

        #region Dependency Property: Command

        public static readonly DependencyProperty WithCommandProperty = DependencyProperty.Register(
            "WithCommand",
            typeof(ICommand),
            typeof(Handle),
            new PropertyMetadata(null));

        public ICommand WithCommand
        {
            get { return (ICommand)GetValue(WithCommandProperty); }
            set { SetValue(WithCommandProperty, value); }
        }

        #endregion

        #region Dependency Property: PassingIn

        public static readonly DependencyProperty PassingInProperty = DependencyProperty.Register(
            "PassingIn",
            typeof(string),
            typeof(Handle),
            new PropertyMetadata(null));

        public string PassingIn
        {
            get { return (string)GetValue(PassingInProperty); }
            set { SetValue(PassingInProperty, value); }
        }

        #endregion

        RoutedEventHandler handler = null;

        public Handle()
        {
            handler = (s, e) =>
            {
                object args = null;
                if (!string.IsNullOrWhiteSpace(PassingIn))
                {
                    object referencedObject = null;

                    if (PassingIn.StartsWith("$args.", StringComparison.OrdinalIgnoreCase))
                    {
                        referencedObject = e;
                    }
                    else if (PassingIn.StartsWith("$this.", StringComparison.OrdinalIgnoreCase))
                    {
                        referencedObject = AssociatedObject;
                    }
                    else if (PassingIn.StartsWith("$source.", StringComparison.OrdinalIgnoreCase))
                    {
                        referencedObject = e.Source;
                    }
                    else if (PassingIn.StartsWith("$originalSource.", StringComparison.OrdinalIgnoreCase))
                    {
                        referencedObject = e.OriginalSource;
                    }

                    if (referencedObject != null)
                    {
                        var indexOfFirstDot = PassingIn.IndexOf('.');

                        //TODO: add support for nested properties Foo.Bar.Property
                        var propertyPath = PassingIn.Substring(indexOfFirstDot + 1).Split('.');
                        var property = propertyPath.First();

                        args = referencedObject.GetType().GetProperty(property).GetValue(referencedObject, null);
                    }
                    else if (PassingIn.Equals("$args", StringComparison.OrdinalIgnoreCase))
                    {
                        args = e;
                    }
                    else if (PassingIn.Equals("$this", StringComparison.OrdinalIgnoreCase))
                    {
                        args = AssociatedObject;
                    }
                    else if (PassingIn.Equals("$source", StringComparison.OrdinalIgnoreCase))
                    {
                        args = e.Source;
                    }
                    else if (PassingIn.Equals("$originalSource", StringComparison.OrdinalIgnoreCase))
                    {
                        args = e.OriginalSource;
                    }
                }

                //to do add support for AutoCommandBinding with MethodFact?
                if (WithCommand.CanExecute(args))
                {
                    WithCommand.Execute(args);
                }
            };
        }

        protected override void OnAttached()
        {
            base.OnAttached();

            AssociatedObject.AddHandler(RoutedEvent, handler);
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();

            AssociatedObject.RemoveHandler(RoutedEvent, handler);
        }
    }
}
