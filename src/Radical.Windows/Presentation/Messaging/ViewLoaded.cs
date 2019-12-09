using System;
using System.Windows;
using Radical.Messaging;

namespace Radical.Windows.Presentation.Messaging
{
    /// <summary>
    /// Domain event that identifies that a view has been loaded.
    /// </summary>
    public class ViewLoaded
    {
		/// <summary>
		/// Initializes a new instance of the <see cref="ViewLoaded" /> class.
		/// </summary>
		/// <param name="view">The view.</param>
		public ViewLoaded(DependencyObject view)
		{
			View = view;
		}

        /// <summary>
        /// Gets the view.
        /// </summary>
        public DependencyObject View { get; private set; }
    }
}
