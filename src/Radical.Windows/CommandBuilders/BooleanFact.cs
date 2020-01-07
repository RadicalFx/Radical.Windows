using System;

namespace Radical.Windows.CommandBuilders
{
    public class BooleanFact
    {
        public Func<bool> FastGetter { get; set; }
        public string Name { get; set; }
    }
}
