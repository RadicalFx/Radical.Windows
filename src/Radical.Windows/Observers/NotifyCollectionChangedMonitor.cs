using Radical.ComponentModel;
using System.Collections.Specialized;

namespace Radical.Observers
{
    public class NotifyCollectionChangedMonitor : AbstractMonitor<INotifyCollectionChanged>
    {
        public static NotifyCollectionChangedMonitor For(INotifyCollectionChanged source)
        {
            return new NotifyCollectionChangedMonitor(source);
        }

        NotifyCollectionChangedEventHandler handler = null;

        public NotifyCollectionChangedMonitor(INotifyCollectionChanged source)
            : base(source)
        {

        }

        public NotifyCollectionChangedMonitor()
            : base()
        {

        }

        public NotifyCollectionChangedMonitor(INotifyCollectionChanged source, IDispatcher dispatcher)
            : base(source, dispatcher)
        {

        }

        public NotifyCollectionChangedMonitor(IDispatcher dispatcher)
            : base(dispatcher)
        {

        }

        protected override void StartMonitoring(object source)
        {
            base.StartMonitoring(source);

            handler = (s, e) => OnChanged();
            Source.CollectionChanged += handler;
        }

        public void Observe(INotifyCollectionChanged source)
        {
            StopMonitoring();
            StartMonitoring(source);
        }

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
