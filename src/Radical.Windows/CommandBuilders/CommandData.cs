using Radical.ComponentModel;
using Radical.ComponentModel.Windows.Input;
using Radical.Reflection;
using System;

namespace Radical.Windows.CommandBuilders
{
    public class CommandData
    {
        public string MethodName;

        public Object DataContext;
        public LateBoundVoidMethod FastDelegate;

        public Fact Fact;
        public BooleanFact BooleanFact;

        public Boolean HasParameter;
        public Type ParameterType;

        public KeyBindingAttribute[] KeyBindings;
        public CommandDescriptionAttribute Description;

        public IMonitor Monitor;
    }
}
