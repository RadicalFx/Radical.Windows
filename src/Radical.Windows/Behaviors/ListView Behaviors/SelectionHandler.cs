using Radical.ComponentModel;
using Radical.Linq;
using System;
using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows.Controls;

namespace Radical.Windows.Behaviors
{
    sealed class SelectionHandler
    {
        ListView owner;
        IList selectedItems;

        NotifyCollectionChangedEventHandler ncceh;
        ListChangedEventHandler lceh;
        SelectionChangedEventHandler sceh;

        public SelectionHandler()
        {
            sceh = (s, e) =>
            {
                /*
                 * La ListView ci notifica che la selezione
                 * è cambiata.
                 * 
                 * Per prima cosa ci sganciamo temporaneamente
                 * dalla gestione delle notifiche in modo da non
                 * innescare un meccanismo ricorsivo infinito
                 */
                Unwire();

                var bag = GetSelectedItemsBag();
                e.RemovedItems.Enumerate(obj =>
               {
                   var item = GetRealItem(obj);
                   if (bag.Contains(item))
                   {
                       bag.Remove(item);
                   }
               });

                e.AddedItems.Enumerate(obj =>
               {
                   var item = GetRealItem(obj);
                   if (!bag.Contains(item))
                   {
                       bag.Add(item);
                   }
               });

                Wire();
            };

            ncceh = (s, e) =>
            {
                Unwire();

                switch (e.Action)
                {
                    case NotifyCollectionChangedAction.Add:
                        {
                            AddToListViewSelection(e.NewItems);
                        }
                        break;

                    case NotifyCollectionChangedAction.Remove:
                        {
                            RemoveFromListViewSelection(e.OldItems);
                        }
                        break;

                    case NotifyCollectionChangedAction.Reset:
                        {
                            ClearListViewSelection();
                            AddToListViewSelection(GetSelectedItemsBag());
                        }
                        break;

                    case NotifyCollectionChangedAction.Move:
                    case NotifyCollectionChangedAction.Replace:
                        //NOP
                        break;

                    default:
                        throw new NotSupportedException();
                }

                Wire();
            };

            lceh = (s, e) =>
            {
                Unwire();

                switch (e.ListChangedType)
                {
                    case ListChangedType.ItemAdded:
                        {
                            var bag = (IEntityView)selectedItems;
                            var item = bag[e.NewIndex];

                            AddToListViewSelection(new[] { item });
                        }
                        break;

                    case ListChangedType.Reset:
                        {
                            ClearListViewSelection();
                            AddToListViewSelection(selectedItems);
                        }
                        break;

                    case ListChangedType.ItemDeleted:
                        {
                            RemoveFromListViewSelectionAtIndex(e.NewIndex);
                        }
                        break;

                    case ListChangedType.ItemChanged:
                    case ListChangedType.ItemMoved:
                    case ListChangedType.PropertyDescriptorAdded:
                    case ListChangedType.PropertyDescriptorChanged:
                    case ListChangedType.PropertyDescriptorDeleted:
                        //NOP
                        break;

                    default:
                        throw new NotSupportedException();
                }

                Wire();
            };
        }

        void ClearListViewSelection()
        {
            switch (owner.SelectionMode)
            {
                case SelectionMode.Extended:
                case SelectionMode.Multiple:
                    owner.SelectedItems.Clear();
                    break;
                case SelectionMode.Single:
                    owner.SelectedItem = null;
                    break;

                default:
                    throw new NotSupportedException(string.Format("Unsupported ListView SelectionMode: {0}", owner.SelectionMode));
            }
        }

        void AddToListViewSelection(IEnumerable items)
        {
            switch (owner.SelectionMode)
            {
                case SelectionMode.Extended:
                case SelectionMode.Multiple:
                    items.Enumerate(o => owner.SelectedItems.Add(o));
                    break;
                case SelectionMode.Single:
                    owner.SelectedItem = items.OfType<object>().FirstOrDefault();
                    break;

                default:
                    throw new NotSupportedException(string.Format("Unsupported ListView SelectionMode: {0}", owner.SelectionMode));
            }
        }

        void RemoveFromListViewSelection(IEnumerable items)
        {
            switch (owner.SelectionMode)
            {
                case SelectionMode.Extended:
                case SelectionMode.Multiple:
                    items.Enumerate(o => owner.SelectedItems.Remove(o));
                    break;
                case SelectionMode.Single:
                    owner.SelectedItem = null;
                    break;

                default:
                    throw new NotSupportedException(string.Format("Unsupported ListView SelectionMode: {0}", owner.SelectionMode));
            }
        }

        void RemoveFromListViewSelectionAtIndex(int index)
        {
            switch (owner.SelectionMode)
            {
                case SelectionMode.Extended:
                case SelectionMode.Multiple:
                    owner.SelectedItems.RemoveAt(index);
                    break;
                case SelectionMode.Single:
                    owner.SelectedItem = null;
                    break;

                default:
                    throw new NotSupportedException(string.Format("Unsupported ListView SelectionMode: {0}", owner.SelectionMode));
            }
        }

        IList GetSelectedItemsBag()
        {
            var ev = selectedItems as IEntityView;
            if (ev != null)
            {
                return ev.DataSource;
            }
            return selectedItems;
        }

        object GetRealItem(object source)
        {
            var eiv = source as IEntityItemView;
            if (eiv != null)
            {
                return eiv.EntityItem;
            }

            return source;
        }

        public void SartSync(ListView owner, IList selectedItems)
        {
            this.owner = owner;
            this.selectedItems = selectedItems;

            Wire();
        }

        public void StopSync()
        {
            Unwire();

            owner = null;
            selectedItems = null;
        }

        bool CanSyncFromSource
        {
            get
            {
                var bag = selectedItems;
                return owner.SelectionMode != SelectionMode.Single && (bag is INotifyCollectionChanged || bag is IEntityView);
            }
        }

        void Wire()
        {
            owner.SelectionChanged += sceh;

            if (CanSyncFromSource)
            {
                var bag = selectedItems;
                if (bag is INotifyCollectionChanged)
                {
                    ((INotifyCollectionChanged)bag).CollectionChanged += ncceh;
                }
                else if (bag is IEntityView)
                {
                    ((IEntityView)bag).ListChanged += lceh;
                }
            }
        }

        void Unwire()
        {
            owner.SelectionChanged -= sceh;
            if (CanSyncFromSource)
            {
                var bag = selectedItems;
                if (bag is INotifyCollectionChanged)
                {
                    ((INotifyCollectionChanged)bag).CollectionChanged -= ncceh;
                }
                else if (bag is IEntityView)
                {
                    ((IEntityView)bag).ListChanged -= lceh;
                }
            }
        }
    }
}
