using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace Radical.Windows.Behaviors
{
    /// <summary>
    /// A behavior that handles a routed event executing a command.
    /// </summary>
    public class Handle : RadicalBehavior<FrameworkElement>
    {
        #region Dependency Property: RoutedEvent

        /// <summary>
        /// Identifies the <see cref="RoutedEvent"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty RoutedEventProperty = DependencyProperty.Register(
            "RoutedEvent",
            typeof(RoutedEvent),
            typeof(Handle),
            new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the routed event to handle.
        /// </summary>
        public RoutedEvent RoutedEvent
        {
            get { return (RoutedEvent)GetValue(RoutedEventProperty); }
            set { SetValue(RoutedEventProperty, value); }
        }

        #endregion

        #region Dependency Property: Command

        /// <summary>
        /// Identifies the <see cref="WithCommand"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty WithCommandProperty = DependencyProperty.Register(
            "WithCommand",
            typeof(ICommand),
            typeof(Handle),
            new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the command executed when the routed event is raised.
        /// </summary>
        public ICommand WithCommand
        {
            get { return (ICommand)GetValue(WithCommandProperty); }
            set { SetValue(WithCommandProperty, value); }
        }

        #endregion

        #region Dependency Property: PassingIn

        /// <summary>
        /// Identifies the <see cref="PassingIn"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty PassingInProperty = DependencyProperty.Register(
            "PassingIn",
            typeof(string),
            typeof(Handle),
            new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the command parameter: <c>$args</c>, <c>$this</c>, <c>$source</c>, or
        /// <c>$originalSource</c>, optionally followed by a property name, e.g. <c>$args.Key</c>.
        /// </summary>
        public string PassingIn
        {
            get { return (string)GetValue(PassingInProperty); }
            set { SetValue(PassingInProperty, value); }
        }

        #endregion

        readonly RoutedEventHandler handler = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="Handle"/> class.
        /// </summary>
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

        /// <inheritdoc/>
        protected override void OnAttached()
        {
            base.OnAttached();

            AssociatedObject.AddHandler(RoutedEvent, handler);
        }

        /// <inheritdoc/>
        protected override void OnDetaching()
        {
            base.OnDetaching();

            AssociatedObject.RemoveHandler(RoutedEvent, handler);
        }
    }
}
