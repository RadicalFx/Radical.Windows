using System;
using System.Linq;

namespace Radical.Windows.Internals
{
    class CommandLine(string[] args)
    {
        static CommandLine _current;
        public static CommandLine GetCurrent()
        {
            if (_current == null)
            {
                _current = new CommandLine(Environment.GetCommandLineArgs());
            }
            return _current;
        } 
        
        const char SEPARATOR = '=';

        /// <summary>
        /// Given a command line argument removes leading / or -, and if any,
        /// removes the argument value.
        /// </summary>
        /// <param name="fullArgument">The full argument.</param>
        /// <returns>Just the argument key.</returns>
        static string Normalize(string fullArgument)
        {
            if (fullArgument.StartsWith("/") || fullArgument.StartsWith("-"))
            {
                fullArgument = fullArgument.Substring(1);
            }

            var idx = fullArgument.IndexOf(SEPARATOR);
            if (idx != -1)
            {
                fullArgument = fullArgument.Substring(0, idx);
            }

            return fullArgument;
        }

        /// <summary>
        /// Determines whether the current command contains the specified argument.
        /// </summary>
        /// <param name="arg">The argument to search for.</param>
        /// <returns>
        ///     <c>true</c> if the current command contains the specified argument; otherwise, <c>false</c>.
        /// </returns>
        public bool Contains(string arg)
        {
            var query = args.Where(s => Normalize(s).Equals(arg, StringComparison.CurrentCultureIgnoreCase));
            return query.Any();
        }
    }
}