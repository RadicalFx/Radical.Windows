using Radical.Windows.Behaviors;
using Radical.Windows.ComponentModel;
using System;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;

namespace Radical.Windows.Markup
{
    /// <summary>
    /// A binding to a command that, once the target is loaded, registers the command input
    /// bindings on the root element.
    /// </summary>
    public class CommandBinding : BindingDecoratorBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CommandBinding" /> class.
        /// </summary>
        public CommandBinding()
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandBinding" /> class.
        /// </summary>
        /// <param name="path">The path.</param>
        public CommandBinding(string path)
            : base(path)
        {

        }

        /// <summary>
        /// This basic implementation just sets a binding on the targeted
        /// <see cref="DependencyObject"/> and returns the appropriate
        /// <see cref="BindingExpressionBase"/> instance.<br/>
        /// All this work is delegated to the decorated <see cref="Binding"/>
        /// instance.
        /// </summary>
        /// <param name="provider">Object that can provide services for the markup
        /// extension.</param>
        /// <returns>
        /// The object value to set on the property where the extension is applied.
        /// In case of a valid binding expression, this is a <see cref="BindingExpressionBase"/>
        /// instance.
        /// </returns>
        public override object ProvideValue(IServiceProvider provider)
        {
            if (IsUsingSharedDependencyProperty(provider))
            {
                return this;
            }

            var b = base.ProvideValue(provider);

            OnProvideValue(provider, b);

            return b;
        }

        /// <summary>
        /// Called when the value is provided, hooks the target lifecycle.
        /// </summary>
        /// <param name="provider">The service provider.</param>
        /// <param name="value">The provided value.</param>
        protected virtual void OnProvideValue(IServiceProvider provider, object value)
        {
            if (!DesignTimeHelper.GetIsInDesignMode())
            {

                if (TryGetTargetItems(provider, out FrameworkElement fe, out DependencyProperty dp))
                {
                    void reh(object s, RoutedEventArgs e)
                    {
                        fe.Loaded -= reh;
                        OnTargetLoaded(fe, dp);
                    }

                    fe.Loaded += reh;

                    fe.DataContextChanged += (s, e) =>
                    {
                        OnDataContextChanged(fe, dp, e.NewValue, e.OldValue);
                    };
                }
                else
                {
                    if (TryGetTargetItems(provider, out FrameworkContentElement fce, out dp))
                    {
                        void reh(object s, RoutedEventArgs e)
                        {
                            fce.Loaded -= reh;
                            OnTargetLoaded(fce, dp);
                        }

                        fce.Loaded += reh;

                        fce.DataContextChanged += (s, e) =>
                        {
                            OnDataContextChanged(fce, dp, e.NewValue, e.OldValue);
                        };
                    }
                }
            }
        }

        /// <summary>
        /// Called when the data context of the target changes.
        /// </summary>
        /// <param name="obj">The target.</param>
        /// <param name="targetProperty">The target property.</param>
        /// <param name="newValue">The new data context.</param>
        /// <param name="oldValue">The old data context.</param>
        protected virtual void OnDataContextChanged(DependencyObject obj, DependencyProperty targetProperty, object newValue, object oldValue)
        {

        }

        /// <summary>
        /// Called when the target is loaded.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="targetProperty">The target property.</param>
        protected virtual void OnTargetLoaded(DependencyObject target, DependencyProperty targetProperty)
        {
            var source = target as ICommandSource;
            var command = GetCommand(target, targetProperty);

            SetInputBindings(target, source, command);
        }

        /// <summary>
        /// Adds the command input bindings to the root element of the given target, so that
        /// they are available regardless of the focused element.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="source">The target as command source.</param>
        /// <param name="command">The command.</param>
        protected virtual void SetInputBindings(DependencyObject target, ICommandSource source, IDelegateCommand command)
        {
            if (source != null && command != null && command.InputBindings != null)
            {
                var rootElement = GetRootElement(target as FrameworkElement);
                foreach (InputBinding ib in command.InputBindings)
                {
                    if (ib.CommandParameter != source.CommandParameter)
                    {
                        ib.CommandParameter = source.CommandParameter;
                    }

                    rootElement.InputBindings.Add(ib);
                }
            }
        }

        /// <summary>
        /// Gets the command currently set on the target property.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="targetProperty">The target property.</param>
        /// <returns>The command, if any.</returns>
        protected virtual IDelegateCommand GetCommand(DependencyObject target, DependencyProperty targetProperty)
        {
            return target.GetValue(targetProperty) as IDelegateCommand;
        }

        /// <summary>
        /// Gets the root element of the given element.
        /// </summary>
        /// <param name="fe">The element.</param>
        /// <returns>The root element.</returns>
        protected virtual FrameworkElement GetRootElement(FrameworkElement fe)
        {
            if (fe.Parent == null)
            {
                return fe;
            }
            else
            {
                return GetRootElement(fe.Parent as FrameworkElement);
            }
        }
    }
}