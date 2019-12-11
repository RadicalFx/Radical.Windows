using Radical.Validation;
using Radical.Windows.Presentation.ComponentModel;
using System;
using System.Windows;

namespace Radical.Windows.Presentation.Services
{
    /// <summary>
    /// Resolves view automatically attaching view models by convention.
    /// </summary>
    class ViewResolver : IViewResolver
    {
        readonly IServiceProvider container;
        readonly IConventionsHandler conventions;
        readonly Action<object> emptyInterceptor = o => { };

        /// <summary>
        /// Initializes a new instance of the <see cref="ViewResolver"/> class.
        /// </summary>
        /// <param name="container">The container.</param>
        /// <param name="conventions">The conventions.</param>
        public ViewResolver( IServiceProvider container, IConventionsHandler conventions )
        {
            Ensure.That( container ).Named( () => container ).IsNotNull();
            Ensure.That( conventions ).Named( () => conventions ).IsNotNull();

            this.container = container;
            this.conventions = conventions;
        }

        /// <summary>
        /// Gets the view of the given type.
        /// </summary>
        /// <param name="viewType"></param>
        /// <returns>
        /// The view instance.
        /// </returns>
        public DependencyObject GetView( Type viewType )
        {
            return GetView( viewType, emptyInterceptor );
        }

        /// <summary>
        /// Gets the view.
        /// </summary>
        /// <typeparam name="T">The type of the view.</typeparam>
        /// <returns>
        /// The view instance.
        /// </returns>
        public T GetView<T>() where T : DependencyObject
        {
            return ( T )GetView( typeof( T ), emptyInterceptor );
        }


        /// <summary>
        /// Gets the view.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="viewModelInterceptor">The view model interceptor.</param>
        /// <returns></returns>
        public T GetView<T>( Action<object> viewModelInterceptor ) where T : DependencyObject
        {
            return ( T )GetView( typeof( T ), viewModelInterceptor );
        }

        /// <summary>
        /// Gets the view.
        /// </summary>
        /// <typeparam name="TView">The type of the view.</typeparam>
        /// <typeparam name="TViewModel">The type of the view model.</typeparam>
        /// <param name="viewModelInterceptor">The view model interceptor.</param>
        /// <returns></returns>
        public TView GetView<TView, TViewModel>( Action<TViewModel> viewModelInterceptor ) where TView : DependencyObject
        {
            return ( TView )GetView( typeof( TView ), o =>
            {
                viewModelInterceptor( ( TViewModel )o );
            } );
        }

        /// <summary>
        /// Gets the view.
        /// </summary>
        /// <param name="viewType">Type of the view.</param>
        /// <param name="viewModelInterceptor">The view model interceptor.</param>
        /// <returns></returns>
        public DependencyObject GetView( Type viewType, Action<object> viewModelInterceptor )
        {
            var view = ( DependencyObject )container.GetService( viewType );

            if ( !conventions.ViewHasDataContext( view, ViewDataContextSearchBehavior.LocalOnly ) )
            {
                var viewModelType = conventions.ResolveViewModelType( viewType );
                if ( viewModelType != null )
                {
                    //we support view(s) without ViewModel

                    var viewModel = container.GetService( viewModelType );

                    viewModelInterceptor( viewModel );

                    conventions.AttachViewToViewModel( view, viewModel );
                    conventions.SetViewDataContext( view, viewModel );
                    if(conventions.ShouldExposeViewModelAsStaticResource(view, viewModel))
                    {
                        conventions.ExposeViewModelAsStaticResource(view, viewModel);
                    }
                }

				//behaviors must be attached regardless of the presence of the view model
				//the AttachViewBehaviors is considered to be idempotent.
				conventions.AttachViewBehaviors( view );
            }

            return view;
        }

		///// <summary>
		///// Releases the given view
		///// </summary>
		///// <param name="view">The view to release.</param>
		//public void Release( DependencyObject view )
		//{
		//	throw new NotImplementedException();
		//}
	}
}