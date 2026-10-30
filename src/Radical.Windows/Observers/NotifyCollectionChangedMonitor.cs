using Radical.ComponentModel;
using System.Collections.Specialized;

namespace Radical.Observers
{
    /// <summary>
    /// A monitor that signals changes when the observed collection raises <see cref="INotifyCollectionChanged.CollectionChanged"/>.
    /// </summary>
    public class NotifyCollectionChangedMonitor : AbstractMonitor<INotifyCollectionChanged>
    {
        /// <summary>
        /// Creates a new monitor for the given collection.
        /// </summary>
        /// <param name="source">The collection to monitor.</param>
        /// <returns>The monitor.</returns>
        public static NotifyCollectionChangedMonitor For(INotifyCollectionChanged source)
        {
            return new NotifyCollectionChangedMonitor(source);
        }

        NotifyCollectionChangedEventHandler handler = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="NotifyCollectionChangedMonitor"/> class.
        /// </summary>
        /// <param name="source">The collection to monitor.</param>
        public NotifyCollectionChangedMonitor(INotifyCollectionChanged source)
            : base(source)
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NotifyCollectionChangedMonitor"/> class.
        /// </summary>
        public NotifyCollectionChangedMonitor()
            : base()
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NotifyCollectionChangedMonitor"/> class.
        /// </summary>
        /// <param name="source">The collection to monitor.</param>
        /// <param name="dispatcher">The dispatcher used to notify changes.</param>
        public NotifyCollectionChangedMonitor(INotifyCollectionChanged source, IDispatcher dispatcher)
            : base(source, dispatcher)
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NotifyCollectionChangedMonitor"/> class.
        /// </summary>
        /// <param name="dispatcher">The dispatcher used to notify changes.</param>
        public NotifyCollectionChangedMonitor(IDispatcher dispatcher)
            : base(dispatcher)
        {

        }

        /// <inheritdoc/>
        protected override void StartMonitoring(object source)
        {
            base.StartMonitoring(source);

            handler = (s, e) => OnChanged();
            Source.CollectionChanged += handler;
        }

        /// <summary>
        /// Stops monitoring the current collection, if any, and starts monitoring the given one.
        /// </summary>
        /// <param name="source">The collection to monitor.</param>
        public void Observe(INotifyCollectionChanged source)
        {
            StopMonitoring();
            StartMonitoring(source);
        }

        /// <inheritdoc/>
        protected override void OnStopMonitoring(bool targetDisposed)
        {
            if (!targetDisposed && WeakSource != null && WeakSource.IsAlive)
            {
                Source.CollectionChanged -= handler;
            }

            handler = null;
        }
    }
}
