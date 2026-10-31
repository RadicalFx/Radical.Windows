using Radical.ComponentModel;
using Radical.Reflection;
using Radical.Windows.ComponentModel;
using System;

namespace Radical.Windows.CommandBuilders
{
    /// <summary>
    /// The data used to build a command bound to a data context method.
    /// </summary>
    public class CommandData
    {
        /// <summary>
        /// Gets or sets the name of the method executed by the command.
        /// </summary>
        public string MethodName { get; set; }
        /// <summary>
        /// Gets or sets the data context that defines the method.
        /// </summary>
        public object DataContext { get; set; }
        /// <summary>
        /// Gets or sets the delegate used to invoke the method.
        /// </summary>
        public LateBoundVoidMethod FastDelegate { get; set; }
        /// <summary>
        /// Gets or sets the delegate used to invoke the method when it returns a <see cref="System.Threading.Tasks.Task"/>; otherwise <c>null</c>.
        /// </summary>
        public LateBoundMethod AsyncFastDelegate { get; set; }
        /// <summary>
        /// Gets or sets the boolean property that determines if the command can be executed, if any.
        /// </summary>
        public BooleanFact BooleanFact { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether the method accepts a parameter.
        /// </summary>
        public bool HasParameter { get; set; }
        /// <summary>
        /// Gets or sets the type of the method parameter, if any.
        /// </summary>
        public Type ParameterType { get; set; }
        /// <summary>
        /// Gets or sets the key bindings defined on the method.
        /// </summary>
        public KeyBindingAttribute[] KeyBindings { get; set; }
        /// <summary>
        /// Gets or sets the command description defined on the method, if any.
        /// </summary>
        public CommandDescriptionAttribute Description { get; set; }
        /// <summary>
        /// Gets or sets the monitor used to re-evaluate the command can execute status.
        /// </summary>
        public IMonitor Monitor { get; set; }
    }
}
