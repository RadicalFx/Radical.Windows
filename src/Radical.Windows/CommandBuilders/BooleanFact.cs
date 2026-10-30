using System;

namespace Radical.Windows.CommandBuilders
{
    /// <summary>
    /// Describes the boolean property that determines if a command can be executed.
    /// </summary>
    public class BooleanFact
    {
        /// <summary>
        /// Gets or sets the delegate used to read the property value.
        /// </summary>
        public Func<bool> FastGetter { get; set; }
        /// <summary>
        /// Gets or sets the name of the property.
        /// </summary>
        public string Name { get; set; }
    }
}
