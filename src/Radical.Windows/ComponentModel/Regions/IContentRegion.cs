using System.Windows;

namespace Radical.Windows.ComponentModel
{
    /// <summary>
    /// A region that holds a single, replaceable, content.
    /// </summary>
    public interface IContentRegion : IRegion
    {
        /// <summary>
        /// Gets or sets the content.
        /// </summary>
        /// <value>The content.</value>
        DependencyObject Content { get; set; }
    }
}
