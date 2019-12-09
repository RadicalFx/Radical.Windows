using System;
using System.Windows;

namespace Radical.Windows.Presentation.ComponentModel
{
    /// <summary>
    /// Handles Presentation conventions.
    /// </summary>
    public interface IConventionsHandler
	{
		/// <summary>
		/// Gets or sets the view model type resolver that can resolve the view model type given the view type.
		/// </summary>
		/// <value>
		/// The view model type resolver.
		/// </value>
        [IgnorePropertyInjectionAttribue]
        Func<Type, Type> ResolveViewModelType { get; set; }

		/// <summary>
		/// Default: Gets or sets the view model type resolver that can resolve the view model type given the view type.
		/// </summary>
		/// <value>
		/// The view model type resolver.
		/// </value>
		[IgnorePropertyInjectionAttribue]
		Func<Type, Type> DefaultResolveViewModelType { get; }

		/// <summary>
		/// Gets or sets the view type resolver that can resolve the view type given the view model type.
		/// </summary>
		/// <value>
		/// The view type resolver.
		/// </value>
        [IgnorePropertyInjectionAttribue]
        Func<Type, Type> ResolveViewType { get; set; }

		/// <summary>
		/// Default: Gets or sets the view type resolver that can resolve the view type given the view model type.
		/// </summary>
		/// <value>
		/// The view type resolver.
		/// </value>
		[IgnorePropertyInjectionAttribue]
		Func<Type, Type> DefaultResolveViewType { get; }

		/// <summary>
		/// Gets or sets the view release handler that is responsible to release views and view models.
		/// </summary>
		/// <value>
		/// The view release handler.
		/// </value>
        [IgnorePropertyInjectionAttribue]
        Action<DependencyObject, ViewReleaseBehavior> ViewReleaseHandler { get; set; }

		/// <summary>
		/// Default: Gets or sets the view release handler that is responsible to release views and view models.
		/// </summary>
		/// <value>
		/// The view release handler.
		/// </value>
		[IgnorePropertyInjectionAttribue]
		Action<DependencyObject, ViewReleaseBehavior> DefaultViewReleaseHandler { get; }

		/// <summary>
		/// Gets or sets the handler that determines if a region manager for the given view should be un-registered, the default behavior is that the region manager should be realsed if the view is not a singleton view.
		/// </summary>
		/// <value>
		/// The un-register region manager handler.
		/// </value>
        [IgnorePropertyInjectionAttribue]
        Func<DependencyObject, bool> ShouldUnregisterRegionManagerOfView { get; set; }

		/// <summary>
		/// Default: Gets or sets the handler that determines if a region manager for the given view should be un-registered, the default behavior is that the region manager should be realsed if the view is not a singleton view.
		/// </summary>
		/// <value>
		/// The un-register region manager handler.
		/// </value>
		[IgnorePropertyInjectionAttribue]
		Func<DependencyObject, bool> DefaultShouldUnregisterRegionManagerOfView { get; }

		/// <summary>
		/// Gets or sets the handler that determines if a view should be relased, the default behavior is that the view is released if not a singleton view.
		/// </summary>
		/// <value>
		/// The view release handler.
		/// </value>
        [IgnorePropertyInjectionAttribue]
        Func<DependencyObject, bool> ShouldReleaseView { get; set; }

		/// <summary>
		/// Default: Gets or sets the handler that determines if a view should be relased, the default behavior is that the view is released if not a singleton view.
		/// </summary>
		/// <value>
		/// The view release handler.
		/// </value>
		[IgnorePropertyInjectionAttribue]
		Func<DependencyObject, bool> DefaultShouldReleaseView { get;  }

		/// <summary>
		/// Gets or sets the handler that determines if a view model should be automatically unsubscribed from all the subscriptions when its view is relased, the default behavior is that the view model is unsubscribed if the view is not a singleton view.
		/// </summary>
		/// <value>
		/// The unsubscribe handler.
		/// </value>
        [IgnorePropertyInjectionAttribue]
        Func<DependencyObject, bool> ShouldUnsubscribeViewModelOnRelease { get; set; }

		/// <summary>
		/// Default: Gets or sets the handler that determines if a view model should be automatically unsubscribed from all the subscriptions when its view is relased, the default behavior is that the view model is unsubscribed if the view is not a singleton view.
		/// </summary>
		/// <value>
		/// The unsubscribe handler.
		/// </value>
		[IgnorePropertyInjectionAttribue]
		Func<DependencyObject, bool> DefaultShouldUnsubscribeViewModelOnRelease { get; }

#if !WINDOWS_PHONE
		/// <summary>
		/// Gets or sets the ViewModel -> window finder that given a ViewModel finds the root Window that hosts that ViewModel.
		/// </summary>
		/// <value>
		/// The window finder.
		/// </value>
        [IgnorePropertyInjectionAttribue]
        Func<object, Window> FindHostingWindowOf { get; set; }

		/// <summary>
		/// Default: Gets or sets the ViewModel -> window finder that given a ViewModel finds the root Window that hosts that ViewModel.
		/// </summary>
		/// <value>
		/// The window finder.
		/// </value>
		[IgnorePropertyInjectionAttribue]
		Func<object, Window> DefaultFindHostingWindowOf { get; }

		/// <summary>
		/// Gets or sets the View -> window finder, that given a View finds the root hosting Window for the given View.
		/// </summary>
		/// <value>
		/// The find window of.
		/// </value>
        [IgnorePropertyInjectionAttribue]
        Func<DependencyObject, Window> FindWindowOf { get; set; }

		/// <summary>
		/// Default: Gets or sets the View -> window finder, that given a View finds the root hosting Window for the given View.
		/// </summary>
		/// <value>
		/// The find window of.
		/// </value>
		[IgnorePropertyInjectionAttribue]
		Func<DependencyObject, Window> DefaultFindWindowOf { get; }
#endif
		
		/// <summary>
		/// Gets or sets the logic that determines if view has data context.
		/// </summary>
		/// <value>
		/// The logic that determines if view has data context.
		/// </value>
        [IgnorePropertyInjectionAttribue]
        Func<DependencyObject, ViewDataContextSearchBehavior, bool> ViewHasDataContext { get; set; }

		/// <summary>
		/// Default: Gets or sets the logic that determines if view has data context.
		/// </summary>
		/// <value>
		/// The logic that determines if view has data context.
		/// </value>
		[IgnorePropertyInjectionAttribue]
		Func<DependencyObject, ViewDataContextSearchBehavior, bool> DefaultViewHasDataContext { get; }

		/// <summary>
		/// Gets or sets the logic that determines if ViewModel should notify the loaded message.
		/// </summary>
		/// <value>
		/// The logic that determines if ViewModel should notify the loaded message.
		/// </value>
        [IgnorePropertyInjectionAttribue]
        Func<DependencyObject, object, bool> ShouldNotifyViewModelLoaded { get; set; }

		/// <summary>
		/// Default: Gets or sets the logic that determines if ViewModel should notify the loaded message.
		/// </summary>
		/// <value>
		/// The logic that determines if ViewModel should notify the loaded message.
		/// </value>
		[IgnorePropertyInjectionAttribue]
		Func<DependencyObject, object, bool> DefaultShouldNotifyViewModelLoaded { get; }

		/// <summary>
		/// Gets or sets the logic that determines if View should notify the loaded message.
		/// </summary>
		/// <value>
		/// The logic that determines if View should notify the loaded message.
		/// </value>
        [IgnorePropertyInjectionAttribue]
        Func<DependencyObject, bool> ShouldNotifyViewLoaded { get; set; }

		/// <summary>
		/// Default: Gets or sets the logic that determines if View should notify the loaded message.
		/// </summary>
		/// <value>
		/// The logic that determines if View should notify the loaded message.
		/// </value>
		[IgnorePropertyInjectionAttribue]
		Func<DependencyObject, bool> DefaultShouldNotifyViewLoaded { get; }

		/// <summary>
		/// Gets or sets the logic that sets the view data context.
		/// </summary>
		/// <value>
		/// The logic that sets the view data context.
		/// </value>
        [IgnorePropertyInjectionAttribue]
        Action<DependencyObject, object> SetViewDataContext { get; set; }

        /// <summary>
        /// Default: Gets or sets the logic that determines if the ViewModel should be added to the View static resources.
        /// </summary>
        /// <value>
        /// The logic that determines if the ViewModel should be added to the View static resources.
        /// </value>
        [IgnorePropertyInjectionAttribue]
        Func<DependencyObject, object, bool> DefaultShouldExposeViewModelAsStaticResource { get; }

        /// <summary>
        /// Gets or sets the logic that determines if the ViewModel should be added to the View static resources.
        /// </summary>
        /// <value>
        /// The logic that determines if the ViewModel should be added to the View static resources.
        /// </value>
        [IgnorePropertyInjectionAttribue]
        Func<DependencyObject, object, bool> ShouldExposeViewModelAsStaticResource { get; set; }

        /// <summary>
        /// Default: Gets or sets the logic that determines that adds the ViewModel to the View static resources.
        /// </summary>
        /// <value>
        /// The logic that determines that adds the ViewModel to the View static resources.
        /// </value>
        [IgnorePropertyInjectionAttribue]
        Action<DependencyObject, object> DefaultExposeViewModelAsStaticResource { get; }

        /// <summary>
        /// Gets or sets the logic that determines that adds the ViewModel to the View static resources.
        /// </summary>
        /// <value>
        /// The logic that determines that adds the ViewModel to the View static resources.
        /// </value>
        [IgnorePropertyInjectionAttribue]
        Action<DependencyObject, object> ExposeViewModelAsStaticResource { get; set; }

        /// <summary>
        /// Default: Gets or sets the logic that sets the view data context.
        /// </summary>
        /// <value>
        /// The logic that sets the view data context.
        /// </value>
        [IgnorePropertyInjectionAttribue]
		Action<DependencyObject, object> DefaultSetViewDataContext { get; }

		/// <summary>
		/// Gets or sets the logic that gets view data context.
		/// </summary>
		/// <value>
		/// The logic that gets view data context.
		/// </value>
        [IgnorePropertyInjectionAttribue]
        Func<DependencyObject, ViewDataContextSearchBehavior, object> GetViewDataContext { get; set; }

		/// <summary>
		/// Default: Gets or sets the logic that gets view data context.
		/// </summary>
		/// <value>
		/// The logic that gets view data context.
		/// </value>
		[IgnorePropertyInjectionAttribue]
		Func<DependencyObject, ViewDataContextSearchBehavior, object> DefaultGetViewDataContext { get; }

		/// <summary>
		/// Gets or sets the attach view to view model handler.
		/// </summary>
		/// <value>
		/// The attach view to view model handler.
		/// </value>
        [IgnorePropertyInjectionAttribue]
        Action<DependencyObject, object> AttachViewToViewModel { get; set; }

		/// <summary>
		/// Default: Gets or sets the attach view to view model handler.
		/// </summary>
		/// <value>
		/// The attach view to view model handler.
		/// </value>
		[IgnorePropertyInjectionAttribue]
		Action<DependencyObject, object> DefaultAttachViewToViewModel { get; }

		/// <summary>
		/// Gets the view of the given view model.
		/// </summary>
		/// <value>
		/// The get view of view model handler.
		/// </value>
        [IgnorePropertyInjectionAttribue]
        Func<object, DependencyObject> GetViewOfViewModel { get; set; }

		/// <summary>
		/// Default: Gets the view of the given view model.
		/// </summary>
		/// <value>
		/// The get view of view model handler.
		/// </value>
		[IgnorePropertyInjectionAttribue]
		Func<object, DependencyObject> DefaultGetViewOfViewModel { get; }

		/// <summary>
		/// Tries to hook closed event of an the element in the visual tree that hosts this given view.
		/// If the hook succedeed the given callback will be called once the hosting element is closed.
		/// </summary>
		/// <returns>The element, that supports closed notifications, in the visual tree that hosts the given view; otherwise <c>null</c>.</returns>
        [IgnorePropertyInjectionAttribue]
        Func<DependencyObject, Action<DependencyObject>, DependencyObject> TryHookClosedEventOfHostOf { get; set; }

		/// <summary>
		/// Default: Tries to hook closed event of an the element in the visual tree that hosts this given view.
		/// If the hook succedeed the given callback will be called once the hosting element is closed.
		/// </summary>
		/// <returns>The element, that supports closed notifications, in the visual tree that hosts the given view; otherwise <c>null</c>.</returns>
		[IgnorePropertyInjectionAttribue]
		Func<DependencyObject, Action<DependencyObject>, DependencyObject> DefaultTryHookClosedEventOfHostOf { get; }

		/// <summary>
		/// Gets or sets the convention that determines if the given FrameworkElement is a hosting view.
		/// </summary>
		/// <value>
		/// The convention that determines if the given FrameworkElement is a hosting view.
		/// </value>
        [IgnorePropertyInjectionAttribue]
        Func<FrameworkElement, bool> IsHostingView { get; set; }

		/// <summary>
		/// Default: Gets or sets the convention that determines if the given FrameworkElement is a hosting view.
		/// </summary>
		/// <value>
		/// The convention that determines if the given FrameworkElement is a hosting view.
		/// </value>
		[IgnorePropertyInjectionAttribue]
		Func<FrameworkElement, bool> DefaultIsHostingView { get; }

		/// <summary>
		/// Gets an opportunity to attach behaviors to the view.
		/// </summary>
		/// <value>
		/// The attach view behaviors handler.
		/// </value>
        [IgnorePropertyInjectionAttribue]
        Action<DependencyObject> AttachViewBehaviors { get; set; }

		/// <summary>
		/// Default: Gets an opportunity to attach behaviors to the view.
		/// </summary>
		/// <value>
		/// The attach view behaviors handler.
		/// </value>
		[IgnorePropertyInjectionAttribue]
		Action<DependencyObject> DefaultAttachViewBehaviors { get; }

		/// <summary>
		/// Gets an opportunity to detach behaviors from the view.
		/// </summary>
		/// <value>
		/// The detach view behaviors handler.
		/// </value>
        [IgnorePropertyInjectionAttribue]
        Action<DependencyObject> DetachViewBehaviors { get; set; }

		/// <summary>
		/// Default: Gets an opportunity to detach behaviors from the view.
		/// </summary>
		/// <value>
		/// The detach view behaviors handler.
		/// </value>
		[IgnorePropertyInjectionAttribue]
		Action<DependencyObject> DefaultDetachViewBehaviors { get; }

        /// <summary>
        /// Gets or sets the default view data context search behavior.
        /// </summary>
        /// <value>
        /// The default view data context search behavior.
        /// </value>
        [IgnorePropertyInjectionAttribue]
        ViewDataContextSearchBehavior DefaultViewDataContextSearchBehavior { get; set; }
	}


    /// <summary>
    /// Determines how the DataContext value is searched on a view.
    /// </summary>
    public enum ViewDataContextSearchBehavior
    {
        /// <summary>
        /// The DataContext dependency property value is retrieved using the ReadLocalValue
        /// method of the view and if the returned value is UnsetLocal null is returned to 
        /// the caller so to ensure that only locally set DataContexts are returned.
        /// </summary>
        LocalOnly = 0,


        /// <summary>
        /// The DataContext property of the View element is returned regardless
        /// of the fact that the dependency property value is inherithed or local.
        /// </summary>
        Legacy = 1
    }

    /// <summary>
    /// Define the View release behavior
    /// </summary>
    public enum ViewReleaseBehavior
    {
        /// <summary>
        /// The view, and thus the ViewModel, is released only if no <see cref="ViewManualReleaseAttribute"/> is defined on the View.
        /// </summary>
        Default = 0,


        /// <summary>
        /// The view, and thus the ViewModel, is released regardless of the <see cref="ViewManualReleaseAttribute"/> that can be defined on the View.
        /// </summary>
        Force = 1
    }
}
