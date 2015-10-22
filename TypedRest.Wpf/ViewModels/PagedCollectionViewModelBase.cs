namespace TypedRest.Wpf.ViewModels
{
    /// <summary>
    /// Base class for building view models operatingon an <see cref="IPagedCollectionEndpoint{TEntity,TElementEndpoint}"/>.
    /// </summary>
    /// <typeparam name="TEntity">The type of entity the <typeparamref name="TEndpoint"/> represents.</typeparam>
    /// <typeparam name="TEndpoint">The specific type of <see cref="IPagedCollectionEndpoint{TEntity,TElementEndpoint}"/> to operate on.</typeparam>
    /// <typeparam name="TElementEndpoint">The specific type of <see cref="IElementEndpoint{TEntity}"/> the <typeparamref name="TEndpoint"/> provides for individual <typeparamref name="TEntity"/>s.</typeparam>
    public abstract class PagedCollectionViewModelBase<TEntity, TEndpoint, TElementEndpoint> : CollectionViewModelBase<TEntity, TEndpoint, TElementEndpoint>
        where TEndpoint : IPagedCollectionEndpoint<TEntity, TElementEndpoint>
        where TElementEndpoint : class, IElementEndpoint<TEntity>
    {
        /// <summary>
        /// Creates a new REST paged collection view model.
        /// </summary>
        /// <param name="endpoint">The REST endpoint this view model operates on.</param>
        protected PagedCollectionViewModelBase(TEndpoint endpoint) : base(endpoint)
        {
        }
    }
}