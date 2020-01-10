using Radical.ComponentModel.Windows.Input;
using Radical.Linq;
using Radical.Observers;
using Radical.Reflection;
using Radical.Windows.Behaviors;
using Radical.Windows.Input;
using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace Radical.Windows.Markup
{
    public class BehaviorCommandBinding : CommandBinding
    {
        protected override void OnProvideValue(IServiceProvider provider, object value)
        {

            if (TryGetTargetItems(provider, out DependencyObject fe, out DependencyProperty dp))
            {
                if (fe is INotifyAttachedOjectLoaded inab)
                {
                    void h(object s, EventArgs e)
                    {
                        inab.AttachedObjectLoaded -= h;
                        OnTargetLoaded(fe, dp);
                    }

                    inab.AttachedObjectLoaded += h;
                }
            }
        }

        protected override void SetInputBindings(DependencyObject target, ICommandSource source, IDelegateCommand command)
        {
            //Not supported ?
        }

        protected override IDelegateCommand GetCommand(DependencyObject target, DependencyProperty targetProperty)
        {
            if (Path != null && target is INotifyAttachedOjectLoaded)
            {
                var dataContext = ((INotifyAttachedOjectLoaded)target)
                    .GetAttachedObject<FrameworkElement>()
                    .DataContext;

                var path = Path.Path;
                var methodName = path.EndsWith("Command") ? path.Remove(path.IndexOf("Command")) : path;
                var factName = "Can" + methodName;
                var method = dataContext.GetType().GetMethod(methodName);

                var def = dataContext.GetType()
                    .GetMethods()
                    .Where(mi => mi.Name.Equals(methodName))
                    .Select(mi =>
                    {
                        var prms = mi.GetParameters();

                        return new
                        {
                            FastDelegate = mi.CreateVoidDelegate(),
                            DataContext = dataContext,
                            HasParameter = prms.Length == 1,
                            ParameterType = prms.Length != 1 ? null : prms[0].ParameterType,
                            KeyBindings = mi.GetAttributes<KeyBindingAttribute>(),
                            Description = method.GetAttribute<CommandDescriptionAttribute>(),
                            Fact = dataContext.GetType()
                                        .GetProperties()
                                        .Where(pi => pi.PropertyType == typeof(bool) && pi.Name.Equals(factName))
                                        .Select(pi => new bool?((bool)pi.GetValue(dataContext, null)))
                                        .SingleOrDefault()
                        };
                    })
                    .SingleOrDefault();

                var text = (def.Description == null) ? string.Empty : def.Description.DisplayText;
                var cmd = DelegateCommand.Create(text)
                    .OnCanExecute(o =>
                    {
                       return def.Fact ?? true;
                    })
                    .OnExecute(o =>
                    {
                        if (def.HasParameter)
                        {
                            var prm = Convert.ChangeType(o, def.ParameterType);
                            def.FastDelegate(def.DataContext, new[] { prm });
                        }
                        else
                        {
                            def.FastDelegate(def.DataContext, null);
                        }
                    });

                if (def.KeyBindings != null)
                {
                    def.KeyBindings
                        .ForEach(kb => cmd.AddKeyGesture(kb.Key, kb.Modifiers));
                }

                if (def.Fact != null)
                {
                    cmd.AddMonitor(PropertyObserver.For((INotifyPropertyChanged)dataContext)
                        .Observe(factName));
                }

                target.SetValue(targetProperty, cmd);

                return cmd;
            }

            return null;
        }
    }
}