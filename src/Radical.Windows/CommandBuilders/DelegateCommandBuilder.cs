using Radical.ComponentModel;
using Radical.ComponentModel.Windows.Input;
using Radical.Diagnostics;
using Radical.Linq;
using Radical.Observers;
using Radical.Reflection;
using Radical.Windows.Behaviors;
using Radical.Windows.Input;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Windows;

namespace Radical.Windows.CommandBuilders
{
    public class DelegateCommandBuilder
    {
        readonly static TraceSource logger = new TraceSource(typeof(DelegateCommandBuilder).FullName);

        public virtual bool CanCreateCommand(PropertyPath path, DependencyObject target)
        {
            if (DesignTimeHelper.GetIsInDesignMode())
            {
                return false;
            }

            return path != null && (target is FrameworkElement || target is FrameworkContentElement);
        }

        public virtual object GetDataContext(DependencyObject target)
        {
            if (target is FrameworkElement)
            {
                return ((FrameworkElement)target).DataContext;
            }
            else
            {
                return ((FrameworkContentElement)target).DataContext;
            }
        }

        /// <summary>
        /// Tries to generate command data.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="dataContext">The data context.</param>
        /// <param name="data">The data.</param>
        /// <returns></returns>
        public virtual bool TryGenerateCommandData(PropertyPath path, object dataContext, out CommandData data)
        {
            var propertyPath = path.Path;
            var nestedProperties = propertyPath
                .Split(new[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
            var propertyLevel = 0;
            while (dataContext != null && propertyLevel < nestedProperties.Length - 1)
            {
                var currentProperty = nestedProperties[propertyLevel];
                var dataContextType = dataContext.GetType();
                var pi = dataContextType.GetProperty(currentProperty);
                if (pi == null)
                {
                    logger.Error("Cannot find any property named: {0}.", currentProperty);
                    dataContext = null;
                    break;
                }
                dataContext = pi.GetValue(dataContext, null);
                propertyLevel++;
                propertyPath = nestedProperties[propertyLevel];
            }

            if (dataContext == null)
            {
                data = null;
                return false;
            }

            var properties = dataContext.GetType().GetProperties();
            var commandData = generateCommandData(dataContext, propertyPath, properties);

            if (commandData == null)
            {
                logger.Warning("Cannot find any method named: {0}, with or without the Command suffix.", propertyPath);
            }

            data = commandData;

            return commandData != null;
        }

        static MethodInfo getMethodToBindTo(object dataContext, string methodName)
        {
            var method = dataContext.GetType()
                .GetMethods()
                .Where(mi => mi.Name.Equals(methodName))
                .SingleOrDefault();

            if (method == null && methodName.EndsWith("Command"))
            {
                method = getMethodToBindTo(dataContext, methodName.Remove(methodName.LastIndexOf("Command")));
            }

            return method;
        }

        static CommandData generateCommandData(object dataContext, string methodName, PropertyInfo[] properties)
        {
            var method = getMethodToBindTo(dataContext, methodName);
            if (method == null)
            {
                return null;
            }

            var factName = string.Concat("Can", method.Name);
            var prms = method.GetParameters();

            return new CommandData()
            {
                MethodName = method.Name,
                DataContext = dataContext,
                FastDelegate = method.CreateVoidDelegate(),

                Fact = properties.Where(pi =>
                {
                    return pi.PropertyType == typeof(Fact) && pi.Name.Equals(factName);
                })
                .Select(pi =>
                {
                    var fact = (Fact)pi.GetValue(dataContext, null);
                    return fact;
                })
                .SingleOrDefault(),

                BooleanFact = properties.Where(pi =>
                {
                    return dataContext is INotifyPropertyChanged
                        && pi.PropertyType == typeof(bool)
                        && pi.Name.Equals(factName);
                })
                .Select(pi => new BooleanFact
                {
                    FastGetter = dataContext.CreateFastPropertyGetter<bool>(pi),
                    Name = pi.Name
                })
                .SingleOrDefault(),

                HasParameter = prms.Length == 1,
                ParameterType = prms.Length != 1 ? null : prms[0].ParameterType,
                KeyBindings = method.GetAttributes<KeyBindingAttribute>(),
                Description = method.GetAttribute<CommandDescriptionAttribute>()
            };
        }

        public virtual IDelegateCommand CreateCommand(CommandData commandData)
        {
            var text = (commandData.Description == null) ?
                        string.Empty :
                        commandData.Description.DisplayText;

            var command = (DelegateCommand)DelegateCommand.Create(text);
            command.SetData(commandData);

            command.OnCanExecute(o =>
           {
               var data = command.GetData<CommandData>();
               if (data.Fact != null)
               {
                   return data.Fact.Eval(o);
               }
               else if (data.BooleanFact != null)
               {
                   var can = data.BooleanFact.FastGetter();
                   return can;
               }

               return true;
           })
                .OnExecute(o =>
               {
                   var data = command.GetData<CommandData>();
                   if (data.HasParameter)
                   {
                       var prm = o;
                       if (o is IConvertible)
                       {
                           prm = Convert.ChangeType(o, data.ParameterType);
                       }

                       data.FastDelegate(data.DataContext, new[] { prm });
                   }
                   else
                   {
                       data.FastDelegate(data.DataContext, null);
                   }
               });

            if (commandData.KeyBindings != null)
            {
                commandData.KeyBindings
                    .ForEach(kb => command.AddKeyGesture(kb.Key, kb.Modifiers));
            }

            IMonitor monitor = null;

            if (commandData.Fact != null)
            {
                monitor = commandData.Fact;
            }
            else if (commandData.BooleanFact != null)
            {
                monitor = PropertyObserver.For((INotifyPropertyChanged)commandData.DataContext)
                        .Observe(commandData.BooleanFact.Name);
            }

            if (command != null && monitor != null)
            {
                command.AddMonitor(monitor);
                commandData.Monitor = monitor;
            }

            return command;
        }
    }
}
