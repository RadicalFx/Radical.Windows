using System;
using Radical.Messaging;

namespace Radical.Windows.Presentation.Messaging
{
#pragma warning disable 0618
    /// <summary>
	/// Domain event that identifies that a view model has been closed.
	/// </summary>
	public class ViewModelClosed
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ViewModelClosed"/> class.
		/// </summary>
		/// <param name="viewModel">The view model.</param>
		public ViewModelClosed(object viewModel )
		{
			ViewModel = viewModel;
		}

		/// <summary>
		/// Gets the view model.
		/// </summary>
		public object ViewModel { get; private set; }
    }

#pragma warning restore 0618
}
