using System.IO;
using System.Reflection;

namespace Radical.Windows.Presentation.Helpers
{
    /// <summary>
    /// An helper calss for environment information.
    /// </summary>
    public class EnvironmentHelper
    {
        /// <summary>
        /// Gets the current directory.
        /// </summary>
        /// <returns>The directory the executable is running from.</returns>
        public static string GetCurrentDirectory()
        {
            return Path.GetDirectoryName( Assembly.GetEntryAssembly().Location );
        }
    }
}
