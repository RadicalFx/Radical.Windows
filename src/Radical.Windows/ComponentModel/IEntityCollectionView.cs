namespace Radical.ComponentModel
{
    using System.Collections.Specialized;
    using System.ComponentModel;

    /// <summary>
    /// An <see cref="IEntityView{T}"/> that is also a WPF <see cref="ICollectionView"/>.
    /// </summary>
    /// <typeparam name="T">The type of the items in the view.</typeparam>
    public interface IEntityCollectionView<T> :
        IEntityView<T>,
        INotifyCollectionChanged,
        ICollectionView,
        ICollectionViewFactory
    {

    }
}