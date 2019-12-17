using Radical.ComponentModel;
using Radical.ComponentModel.Windows.Input;
using Radical.Reflection;
using System;

namespace Radical.Windows.CommandBuilders
{
    public class CommandData
    {
        public string MethodName;

        public object DataContext;
        public LateBoundVoidMethod FastDelegate;

        public BooleanFact BooleanFact;

        public bool HasParameter;
        public Type ParameterType;

        public KeyBindingAttribute[] KeyBindings;
        public CommandDescriptionAttribute Description;

        public IMonitor Monitor;
    }
}
