using System;
using System.Windows;
using System.Windows.Controls;
using Radical.Windows.Presentation.ComponentModel;
using Radical.Conversions;
using System.ComponentModel;

namespace Radical.Windows.Presentation.Regions
{
	/// <summary>
	/// An content region hosted by a ContentPresenter.
	/// </summary>
	public sealed class ContentPresenterRegion : ContentRegion<ContentPresenter>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ContentPresenterRegion"/> class.
		/// </summary>
		public ContentPresenterRegion()
		{

		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ContentPresenterRegion"/> class.
		/// </summary>
		/// <param name="name">The name.</param>
		public ContentPresenterRegion(string name )
			: base( name )
		{

		}

		/// <summary>
		/// Called to get content.
		/// </summary>
		/// <returns></returns>
		protected override DependencyObject OnGetContent()
		{
			return Element.Content as DependencyObject;
		}

		/// <summary>
		/// Called to set new content.
		/// </summary>
		/// <param name="view">The view.</param>
		/// <param name="args">The cancel even arguments.</param>
		protected override void OnSetContent( DependencyObject view, CancelEventArgs args )
		{
			var previous = Element.Content as DependencyObject;


#if !SILVERLIGHT

			if ( previous != null )
			{
				RegionService.Conventions
                    .GetViewDataContext( previous, RegionService.Conventions.DefaultViewDataContextSearchBehavior )
					.As<IExpectViewClosingCallback>( i => i.OnViewClosing( args ) );
			}

#endif

			if ( !args.Cancel )
			{
				Element.Content = view;
			}
		}
	}
}
