﻿using Radical.ComponentModel.Messaging;
using Radical.Diagnostics;
using Radical.Reflection;
using Radical.Validation;
using Radical.Windows.ComponentModel;
using Radical.Windows.Messaging;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;

namespace Radical.Windows.Behaviors
{
    /// <summary>
    /// Wires the window life-cycle to a view model that requires life-cycle notifications.
    /// </summary>
    public class WindowLifecycleNotificationsBehavior : RadicalBehavior<Window>
    {
        static readonly TraceSource logger = new TraceSource( typeof( WindowLifecycleNotificationsBehavior ).FullName );

        readonly IMessageBroker broker;
        readonly IConventionsHandler conventions;
        readonly RoutedEventHandler loaded = null;
        readonly EventHandler activated = null;
        readonly EventHandler rendered = null;
        readonly EventHandler closed = null;
        readonly CancelEventHandler closing = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="WindowLifecycleNotificationsBehavior"/> class.
        /// </summary>
        /// <param name="broker">The broker.</param>
        /// <param name="conventions">The conventions handler.</param>
        public WindowLifecycleNotificationsBehavior( IMessageBroker broker, IConventionsHandler conventions )
        {
            Ensure.That( broker ).Named( () => broker ).IsNotNull();
            Ensure.That( conventions ).Named( () => conventions ).IsNotNull();

            this.broker = broker;
            this.conventions = conventions;

            if( !DesignTimeHelper.GetIsInDesignMode() )
            {
                logger.Debug( "We are not running within a designer." );
                logger.Debug( "Ready to attach events." );

                loaded = ( s, e ) =>
                {
                    logger.Debug( "Loaded event raised." );
                    Ensure.That( AssociatedObject ).Named( "AssociatedObject" ).IsNotNull();

                    var view = AssociatedObject;
                    var dc = this.conventions.GetViewDataContext( view, this.conventions.DefaultViewDataContextSearchBehavior );

                    if( this.conventions.ShouldNotifyViewModelLoaded( view, dc ) )
                    {
                        logger.Debug( "ShouldNotifyViewModelLoaded -> true." );

                        this.broker.Broadcast( this, new ViewModelLoaded( dc ) );

                        logger.Debug( "Message broadcasted." );
                    }
                    
                    if ( this.conventions.ShouldNotifyViewLoaded( view ) )
                    {
                        logger.Debug( "ShouldNotifyViewLoaded -> true." );

                        this.broker.Broadcast( this, new ViewLoaded( view ));

                        logger.Debug( "Message broadcasted." );
                    }

                    if (dc is IExpectViewLoadedCallback temp)
                    {
                        logger.Debug("DataContext is IExpectViewLoadedCallback.");

                        temp.OnViewLoaded();

                        logger.Debug("DataContext.OnViewLoaded() invoked.");
                    }
                };

                logger.Debug( "Loaded event attached." );

                activated = ( s, e ) =>
                {
                    var view = AssociatedObject;
                    var dc = this.conventions.GetViewDataContext( view, this.conventions.DefaultViewDataContextSearchBehavior );

                    if ( dc != null && dc.GetType().IsAttributeDefined<NotifyActivatedAttribute>() )
                    {
                        this.broker.Broadcast( this, new ViewModelActivated( dc ) );
                    }

                    if (dc is IExpectViewActivatedCallback temp)
                    {
                        logger.Debug("DataContext is IExpectViewActivatedCallback.");

                        temp.OnViewActivated();

                        logger.Debug("DataContext.OnViewActivated() invoked.");
                    }
                };

                logger.Debug( "Activated event attached." );

                rendered = ( s, e ) =>
                {
                    logger.Debug( "Rendered event raised." );

                    var view = AssociatedObject;
                    var dc = this.conventions.GetViewDataContext( view, this.conventions.DefaultViewDataContextSearchBehavior );

                    if( dc != null && dc.GetType().IsAttributeDefined<NotifyShownAttribute>() )
                    {
                        this.broker.Broadcast( this, new ViewModelShown( dc ) );
                    }

                    if (dc is IExpectViewShownCallback temp)
                    {
                        logger.Debug("DataContext is IExpectViewShownCallback.");

                        temp.OnViewShown();

                        logger.Debug("DataContext.OnViewShown() invoked.");
                    }
                };


                logger.Debug( "Rendered event attached." );

                closed = ( s, e ) =>
                {
                    logger.Debug( "Closed event raised." );

                    var view = AssociatedObject;
                    var dc = this.conventions.GetViewDataContext( view, this.conventions.DefaultViewDataContextSearchBehavior );

                    if ( dc != null && dc.GetType().IsAttributeDefined<NotifyClosedAttribute>() )
                    {
                        this.broker.Broadcast( this, new ViewModelClosed(  dc ) );
                    }

                    if (dc is IExpectViewClosedCallback temp)
                    {
                        logger.Debug("DataContext is IExpectViewClosedCallback.");

                        temp.OnViewClosed();

                        logger.Debug("DataContext.OnViewClosed() invoked.");
                    }

                    if (this.conventions.ShouldReleaseView(view))
                    {
                        this.conventions.ViewReleaseHandler(view, ViewReleaseBehavior.Default);
                    }
                };


                logger.Debug( "Closed event attached." );

                closing = ( s, e ) =>
                {
                    logger.Debug( "Closing event raised." );

                    var view = AssociatedObject;
                    var dc = this.conventions.GetViewDataContext( view, this.conventions.DefaultViewDataContextSearchBehavior );

                    if (dc is IExpectViewClosingCallback temp)
                    {
                        logger.Debug("DataContext is IExpectViewClosingCallback.");

                        temp.OnViewClosing(e);

                        logger.Debug("DataContext.OnViewClosing() invoked.");
                    }
                };


                logger.Debug( "Closing event attached." );
            }
            else
            {
                logger.Information("We are running within a designer.");
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
                AssociatedObject.Activated += activated;
                AssociatedObject.ContentRendered += rendered;
                AssociatedObject.Closing += closing;
                AssociatedObject.Closed += closed;
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
                AssociatedObject.Activated -= activated;
                AssociatedObject.ContentRendered -= rendered;
                AssociatedObject.Closing -= closing;
                AssociatedObject.Closed -= closed;
            }
            base.OnDetaching();
        }
    }
}
