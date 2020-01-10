using Radical.ComponentModel;
using Radical.Linq;
using Radical.Model;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Runtime.Serialization;

namespace Radical.Windows.Model
{
    public class ObservableEntityCollection<T> :
        MementoEntityCollection<T>,
        INotifyCollectionChanged,
        INotifyPropertyChanged
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ObservableEntityCollection&lt;T&gt;"/> class.
        /// </summary>
        public ObservableEntityCollection()
            : base()
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ObservableEntityCollection&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="capcity">The capcity.</param>
        public ObservableEntityCollection(int capcity)
            : base(capcity)
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ObservableEntityCollection&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="collection">The collection.</param>
        public ObservableEntityCollection(IEnumerable<T> collection)
            : base(collection)
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ObservableEntityCollection&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="info">The info.</param>
        /// <param name="context">The context.</param>
        protected ObservableEntityCollection(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {

        }

        protected override void OnAddCompleted(int index, T value)
        {
            base.OnAddCompleted(index, value);

            OnPropertyChanged(() => Count);
        }

        protected override void OnRemoveCompleted(T value, int index)
        {
            base.OnRemoveCompleted(value, index);

            OnPropertyChanged(() => Count);
        }

        protected override void OnClearCompleted(IEnumerable<T> clearedItems)
        {
            base.OnClearCompleted(clearedItems);

            OnPropertyChanged(() => Count);
        }

        protected override void OnInsertCompleted(int index, T value)
        {
            base.OnInsertCompleted(index, value);

            OnPropertyChanged(() => Count);
        }

        protected override void OnDeserializationCompleted(SerializationInfo info, StreamingContext context)
        {
            OnPropertyChanged(() => Count);

            base.OnDeserializationCompleted(info, context);
        }

        public override void EndInit(bool notify)
        {
            base.EndInit(notify);

            if (notify)
            {
                foreach (var e in propertyChangesNotificationQueue)
                {
                    OnPropertyChanged(new PropertyChangedEventArgs(e));
                }

                propertyChangesNotificationQueue.Clear();
            }
        }

        readonly HashSet<string> propertyChangesNotificationQueue = new HashSet<string>();
        static readonly object propertyChangedEventKey = new object();

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged
        {
            add { Events.AddHandler(propertyChangedEventKey, value); }
            remove { Events.RemoveHandler(propertyChangedEventKey, value); }
        }

        /// <summary>
        /// Raises the <see cref="E:PropertyChanged"/> event.
        /// </summary>
        /// <param name="property">The property.</param>
        protected void OnPropertyChanged<TValue>(Expression<Func<TValue>> property)
        {
            var propertyName = property.GetMemberName();
            OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Raises the <see cref="E:PropertyChanged"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.ComponentModel.PropertyChangedEventArgs"/> instance containing the event data.</param>
        protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            EnsureNotDisposed();
            if (!IsInitializing)
            {
                if (Events[propertyChangedEventKey] is PropertyChangedEventHandler handler)
                {
                    handler(this, e);
                }
            }
            else
            {
                if (!propertyChangesNotificationQueue.Contains(e.PropertyName))
                {
                    propertyChangesNotificationQueue.Add(e.PropertyName);
                }
            }
        }

        ///// <summary>
        ///// Called in order to create a new view.
        ///// Overrdie this memeber to customize the creation process.
        ///// </summary>
        ///// <returns>An instance of a view.</returns>
        //protected override IEntityView<T> OnCreateView()
        //{
        //    return new EntityCollectionView<T>( this );
        //}

        protected override void OnAddRange(IEnumerable<T> rangeToAdd)
        {
            base.OnAddRange(rangeToAdd);

            BeginInit();
        }

        protected override void OnAddRangeCompleted(IEnumerable<T> addedRange)
        {
            base.OnAddRangeCompleted(addedRange);

            OnPropertyChanged(() => Count);

            EndInit();
        }

        /// <summary>
        /// Raises the <see cref="E:CollectionChanged"/> event.
        /// </summary>
        /// <param name="e">The <see cref="Radical.ComponentModel.CollectionChangedEventArgs&lt;T&gt;"/> instance containing the event data.</param>
        protected override void OnCollectionChanged(CollectionChangedEventArgs<T> e)
        {
            base.OnCollectionChanged(e);

            if (!IsInitializing)
            {
                /*
                 * Non facciamo altro che mappare il "nostro" evento
                 * CollectionChanged su INotifyCollectionChanged.CollectionChanged
                 */
                switch (e.ChangeType)
                {
                    case CollectionChangeType.SortChanged:
                        OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
                        break;

                    case CollectionChangeType.ItemAdded:
                        OnCollectionChanged(
                            new NotifyCollectionChangedEventArgs(
                                NotifyCollectionChangedAction.Add,
                                e.Item,
                                e.Index));
                        break;

                    case CollectionChangeType.ItemRemoved:
                        OnCollectionChanged(
                            new NotifyCollectionChangedEventArgs(
                                NotifyCollectionChangedAction.Remove,
                                e.Item,
                                e.Index));
                        break;

                    case CollectionChangeType.Reset:
                        OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
                        break;

                    case CollectionChangeType.ItemMoved:
                        OnCollectionChanged(
                            new NotifyCollectionChangedEventArgs(
                                NotifyCollectionChangedAction.Move,
                                e.Item,
                                e.Index,
                                e.OldIndex));
                        break;

                    case CollectionChangeType.ItemChanged:
                    case CollectionChangeType.ItemReplaced:

                        var newValue = this[e.Index];

                        OnCollectionChanged(
                            new NotifyCollectionChangedEventArgs(
                                NotifyCollectionChangedAction.Replace,
                                newValue,
                                e.Item,
                                e.Index));
                        break;

                    /*
                     * Questo non dovrebbe mai esserci quindi ha senso 
                     * che se arriva ci sia una sonora exception
                     */
                    case CollectionChangeType.None:
                    default:
                        throw new NotSupportedException();
                }
            }
        }

        static readonly object notifyCollectionChangedEventKey = new object();

        /// <summary>
        /// Occurs when this collection changes.
        /// </summary>
        event NotifyCollectionChangedEventHandler INotifyCollectionChanged.CollectionChanged
        {
            add { Events.AddHandler(notifyCollectionChangedEventKey, value); }
            remove { Events.RemoveHandler(notifyCollectionChangedEventKey, value); }
        }

        /// <summary>
        /// Raises the <see cref="E:CollectionChanged"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.Collections.Specialized.NotifyCollectionChangedEventArgs"/> instance containing the event data.</param>
        protected virtual void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            if (Events[notifyCollectionChangedEventKey] is NotifyCollectionChangedEventHandler handler)
            {
                handler(this, e);
            }
        }
    }
}