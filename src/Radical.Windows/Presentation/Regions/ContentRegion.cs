using Radical.Windows.Presentation.ComponentModel;
using System.ComponentModel;
using System.Windows;

namespace Radical.Windows.Presentation.Regions
{
    /// <summary>
    /// A base abstract implementation of the <see cref="IContentRegion"/>.
    /// </summary>
    /// <typeparam name="T">The type of the element that hosts this region.</typeparam>
    public abstract class ContentRegion<T> :
		Region<T>,
		IContentRegion
		where T : FrameworkElement
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ContentRegion&lt;T&gt;"/> class.
		/// </summary>
		protected ContentRegion()
		{

		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ContentRegion&lt;T&gt;"/> class.
		/// </summary>
		/// <param name="name">The name.</param>
		protected ContentRegion(string name )
			: base( name )
		{

		}

		/// <summary>
		/// Closes this instance.
		/// </summary>
		public override void Shutdown()
		{
			NotifyClosedAndEnsureRelease( Content );
		}

		/// <summary>
		/// Gets or sets the content.
		/// </summary>
		/// <value>
		/// The content.
		/// </value>
		public DependencyObject Content
		{
			get { return OnGetContent(); }
			set
			{
				if ( value != Content )
				{
					var args = new CancelEventArgs()
					{
						Cancel = false
					};

					var previous = Content;
					OnSetContent( value, args );
					if ( !args.Cancel )
					{
						OnContentSet( value, previous );
					}
				}
			}
		}

		/// <summary>
		/// Called to get content.
		/// </summary>
		/// <returns></returns>
		protected abstract DependencyObject OnGetContent();

		/// <summary>
		/// Called to set new content.
		/// </summary>
		/// <param name="view">The view.</param>
		/// <param name="args">The cancel even arguments.</param>
		protected abstract void OnSetContent( DependencyObject view, CancelEventArgs args );

		/// <summary>
		/// Called when content has been set.
		/// </summary>
		/// <param name="actual">The actual.</param>
		/// <param name="previous">The previous.</param>
		protected virtual void OnContentSet( DependencyObject actual, DependencyObject previous )
		{
			NotifyClosedAndEnsureRelease( previous );
		}
	}
}
