using Radical.ComponentModel.Windows.Input;
using Radical.Windows.Behaviors;
using System;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;

namespace Radical.Windows.Markup
{
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

        protected virtual void OnProvideValue(IServiceProvider provider, object value)
        {
            if (!DesignTimeHelper.GetIsInDesignMode())
            {
                FrameworkElement fe;
                DependencyProperty dp;

                if (TryGetTargetItems<FrameworkElement>(provider, out fe, out dp))
                {
                    RoutedEventHandler reh = null;
                    reh = (s, e) =>
                    {
                        fe.Loaded -= reh;
                        OnTargetLoaded(fe, dp);
                    };

                    fe.Loaded += reh;

                    fe.DataContextChanged += (s, e) =>
                    {
                        OnDataContextChanged(fe, dp, e.NewValue, e.OldValue);
                    };
                }
#if !SILVERLIGHT
                else
                {
                    FrameworkContentElement fce;
                    if (TryGetTargetItems<FrameworkContentElement>(provider, out fce, out dp))
                    {
                        RoutedEventHandler reh = null;
                        reh = (s, e) =>
                        {
                            fce.Loaded -= reh;
                            OnTargetLoaded(fce, dp);
                        };

                        fce.Loaded += reh;

                        fce.DataContextChanged += (s, e) =>
                        {
                            OnDataContextChanged(fce, dp, e.NewValue, e.OldValue);
                        };
                    }
                }
#endif
            }
        }

        protected virtual void OnDataContextChanged(DependencyObject obj, DependencyProperty targetProperty, object newValue, object oldValue)
        {

        }

        protected virtual void OnTargetLoaded(DependencyObject target, DependencyProperty targetProperty)
        {
            var source = target as ICommandSource;
            var command = GetCommand(target, targetProperty);

            SetInputBindings(target, source, command);
        }

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

        protected virtual IDelegateCommand GetCommand(DependencyObject target, DependencyProperty targetProperty)
        {
            return target.GetValue(targetProperty) as IDelegateCommand;
        }

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