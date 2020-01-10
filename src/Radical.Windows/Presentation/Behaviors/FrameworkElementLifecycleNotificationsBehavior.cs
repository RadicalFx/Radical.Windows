using Radical.ComponentModel.Messaging;
using Radical.Windows.Behaviors;
using Radical.Windows.Presentation.ComponentModel;
using Radical.Windows.Presentation.Messaging;
using System.Windows;

namespace Radical.Windows.Presentation.Behaviors
{
    /// <summary>
    /// Wires the FrameworkElement lifecycle to a view model that requires lifecycle notifications.
    /// </summary>
    public class FrameworkElementLifecycleNotificationsBehavior : RadicalBehavior<FrameworkElement>
    {
        readonly IMessageBroker broker;
        readonly IConventionsHandler conventions;

        RoutedEventHandler loaded = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="FrameworkElementLifecycleNotificationsBehavior"/> class.
        /// </summary>
        /// <param name="broker">The broker.</param>
        /// <param name="conventions">The conventions handler.</param>
        public FrameworkElementLifecycleNotificationsBehavior( IMessageBroker broker, IConventionsHandler conventions )
        {
            this.broker = broker;
            this.conventions = conventions;

            if ( !DesignTimeHelper.GetIsInDesignMode() )
            {
                loaded = ( s, e ) =>
                {
                    var view = AssociatedObject;
                    var dc = this.conventions.GetViewDataContext( view, this.conventions.DefaultViewDataContextSearchBehavior );

                    if ( this.conventions.ShouldNotifyViewModelLoaded( view, dc ) )
                    {
                        this.broker.Broadcast( this, new ViewModelLoaded( dc ) );
                    }

                    if ( this.conventions.ShouldNotifyViewLoaded( view ) )
                    {
                        this.broker.Broadcast( this, new ViewLoaded( view ) );
                    }

                    if (dc is IExpectViewLoadedCallback temp)
                    {
                        temp.OnViewLoaded();
                    }
                };
            }
        }

        /// <summary>
        /// Called after the behavior is attached to an AssociatedObject.
        /// </summary>
        protected override void OnAttached()
        {
            base.OnAttached();

            if ( !DesignTimeHelper.GetIsInDesignMode() )
            {
                AssociatedObject.Loaded += loaded;
            }
        }

        /// <summary>
        /// Called when the behavior is being detached from its AssociatedObject, but before it has actually occurred.
        /// </summary>
        protected override void OnDetaching()
        {
            if ( !DesignTimeHelper.GetIsInDesignMode() )
            {
                AssociatedObject.Loaded -= loaded;
            }
            base.OnDetaching();
        }
    }
}
