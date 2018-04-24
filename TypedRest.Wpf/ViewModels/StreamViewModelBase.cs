using Caliburn.Micro;

namespace TypedRest.Wpf.ViewModels
{
    /// <summary>
    /// Base class for building view models operating on an <see cref="IStreamEndpoint{TEntity,TElementEndpoint}"/>.
    /// </summary>
    /// <typeparam name="TEntity">The type of entity the <typeparamref name="TEndpoint"/> represents.</typeparam>
    /// <typeparam name="TEndpoint">The specific type of <see cref="IStreamEndpoint{TEntity,TElementEndpoint}"/> to operate on.</typeparam>
    /// <typeparam name="TElementEndpoint">The specific type of <see cref="IElementEndpoint{TEntity}"/> the <typeparamref name="TEndpoint"/> provides for individual <typeparamref name="TEntity"/>s.</typeparam>
    public abstract class StreamViewModelBase<TEntity, TEndpoint, TElementEndpoint> : CollectionViewModelBase<TEntity, TEndpoint, TElementEndpoint>
        where TEndpoint : class, IStreamEndpoint<TEntity, TElementEndpoint>
        where TElementEndpoint : class, IEndpoint
    {
        /// <summary>
        /// Creates a new REST paged collection view model.
        /// </summary>
        /// <param name="endpoint">The REST endpoint this view model operates on.</param>
        /// <param name="eventAggregator">Used to send refresh notifications.</param>
        protected StreamViewModelBase(TEndpoint endpoint, IEventAggregator eventAggregator)
            : base(endpoint, eventAggregator)
        {
            CanCreate = false;
            CanOpenElement = false;
        }
    }
}
