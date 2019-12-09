using System;

namespace Radical.Windows.Presentation.ComponentModel
{
	/// <summary>
	/// Defines a view.
	/// </summary>
	[Obsolete( "Use the behaviors via messaging." )]
	public interface IView
	{
        /// <summary>
        /// Gets or sets the data context.
        /// </summary>
        /// <value>
        /// The data context.
        /// </value>
        object DataContext { get; set; }
	}
}
