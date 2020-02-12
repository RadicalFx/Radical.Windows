using System;

namespace Radical.Windows.ComponentModel
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class CommandDescriptionAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CommandDescriptionAttribute"/> class.
        /// </summary>
        /// <param name="displayText">The display text.</param>
        public CommandDescriptionAttribute(string displayText)
        {
            DisplayText = displayText;
        }

        /// <summary>
        /// Gets the command display text.
        /// </summary>
        /// <value>The display text.</value>
        public virtual string DisplayText { get; private set; }
    }
}
