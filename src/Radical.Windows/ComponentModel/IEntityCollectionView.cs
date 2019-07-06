namespace Radical.ComponentModel
{
    using System.Collections.Specialized;
    using System.ComponentModel;

    public interface IEntityCollectionView<T> :
        IEntityView<T>,
        INotifyCollectionChanged,
        ICollectionView,
        /* IEditableCollectionView, */
        ICollectionViewFactory
    {

    }
}