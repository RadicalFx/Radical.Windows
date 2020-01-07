using Radical.ComponentModel;
using Radical.ComponentModel.Windows.Input;
using Radical.Reflection;
using System;

namespace Radical.Windows.CommandBuilders
{
    public class CommandData
    {
        public string MethodName { get; set; }
        public object DataContext { get; set; }
        public LateBoundVoidMethod FastDelegate { get; set; }
        public BooleanFact BooleanFact { get; set; }
        public bool HasParameter { get; set; }
        public Type ParameterType { get; set; }
        public KeyBindingAttribute[] KeyBindings { get; set; }
        public CommandDescriptionAttribute Description { get; set; }
        public IMonitor Monitor { get; set; }
    }
}
