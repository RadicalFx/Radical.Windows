﻿using Radical.ComponentModel;
using System;
using System.Windows;

namespace Radical.Windows
{
    /// <summary>
    /// A WeakEventManager for the MontiorChanged event.
    /// </summary>
    public sealed class MonitorChangedWeakEventManager : WeakEventManager
    {
        static MonitorChangedWeakEventManager GetCurrentManager()
        {
            var mt = typeof(MonitorChangedWeakEventManager);

            var manager = (MonitorChangedWeakEventManager)GetCurrentManager(mt);
            if (manager == null)
            {
                manager = new MonitorChangedWeakEventManager();
                SetCurrentManager(mt, manager);
            }

            return manager;
        }

        /// <summary>
        /// Adds the listener.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="listener">The listener.</param>
        public static void AddListener(IMonitor source, IWeakEventListener listener)
        {
            GetCurrentManager()
                .ProtectedAddListener(source, listener);
        }

        /// <summary>
        /// Removes the listener.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="listener">The listener.</param>
        public static void RemoveListener(IMonitor source, IWeakEventListener listener)
        {
            GetCurrentManager()
                .ProtectedRemoveListener(source, listener);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MonitorChangedWeakEventManager"/> class.
        /// </summary>
        private MonitorChangedWeakEventManager()
        {

        }

        /// <summary>
        /// Called in order to deliver the Changed event.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="args">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        void OnChanged(object sender, EventArgs args)
        {
            DeliverEvent(sender, args);
        }

        /// <summary>
        /// When overridden in a derived class, starts listening for the event being managed. After <see cref="M:System.Windows.WeakEventManager.StartListening(System.Object)"/>  is first called, the manager should be in the state of calling <see cref="M:System.Windows.WeakEventManager.DeliverEvent(System.Object,System.EventArgs)"/> or <see cref="M:System.Windows.WeakEventManager.DeliverEventToList(System.Object,System.EventArgs,System.Windows.WeakEventManager.ListenerList)"/> whenever the relevant event from the provided source is handled.
        /// </summary>
        /// <param name="source">The source to begin listening on.</param>
        protected override void StartListening(object source)
        {
            if (source is IMonitor trigger)
            {
                trigger.Changed += OnChanged;
            }
        }

        /// <summary>
        /// When overridden in a derived class, stops listening on the provided source for the event being managed.
        /// </summary>
        /// <param name="source">The source to stop listening on.</param>
        protected override void StopListening(object source)
        {
            if (source is IMonitor trigger)
            {
                trigger.Changed -= OnChanged;
            }
        }
    }
}
