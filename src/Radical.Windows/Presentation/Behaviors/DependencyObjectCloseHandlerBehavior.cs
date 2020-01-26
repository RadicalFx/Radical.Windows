using Microsoft.Xaml.Behaviors;
using Radical.ComponentModel.Messaging;
using Radical.Windows.ComponentModel;
using Radical.Windows.Presentation.Messaging;
using System.Windows;

namespace Radical.Windows.Presentation.Behaviors
{
    /// <summary>
    /// Special behavior to handle view close requests.
    /// </summary>
    public class DependencyObjectCloseHandlerBehavior : Behavior<DependencyObject>
    {
        readonly IMessageBroker broker;
        readonly IConventionsHandler conventions;

        /// <summary>
        /// Initializes a new instance of the <see cref="DependencyObjectCloseHandlerBehavior"/> class.
        /// </summary>
        /// <param name="broker">The broker.</param>
        /// <param name="conventions">The conventions.</param>
        public DependencyObjectCloseHandlerBehavior( IMessageBroker broker, IConventionsHandler conventions )
        {
            this.broker = broker;
            this.conventions = conventions;
        }

        /// <summary>
        /// Called after the behavior is attached to an AssociatedObject.
        /// </summary>
        protected override void OnAttached()
        {
            base.OnAttached();

            var view = AssociatedObject;
            broker.Subscribe<CloseViewRequest>( this, InvocationModel.Safe, ( s, m ) =>
            {
                var dc = conventions.GetViewDataContext( view, conventions.DefaultViewDataContextSearchBehavior );
                if ( m.ViewOwner == dc )
                {
                    var w = conventions.FindHostingWindowOf( m.ViewOwner );
                    if ( w != null )
                    {
                        if ( m.DialogResult.HasValue )
                        {
                            w.DialogResult = m.DialogResult;
                        }
                        w.Close();
                    }
                }
            } );
        }

        /// <summary>
        /// Called when the behavior is being detached from its AssociatedObject,
        /// but before it has actually occurred.
        /// </summary>
        protected override void OnDetaching()
        {
            if ( broker != null )
            {
                broker.Unsubscribe( this );
            }

            base.OnDetaching();
        }
    }
}