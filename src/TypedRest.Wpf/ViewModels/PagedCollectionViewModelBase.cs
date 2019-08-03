using System.Net.Http.Headers;
using System.Threading.Tasks;
using Caliburn.Micro;
using TypedRest.Endpoints;
using TypedRest.Endpoints.Generic;

namespace TypedRest.Wpf.ViewModels
{
    /// <summary>
    /// Base class for building view models operatingon an <see cref="ICollectionEndpoint{TEntity,TElementEndpoint}"/> using pagination.
    /// </summary>
    /// <typeparam name="TEntity">The type of entity the <typeparamref name="TEndpoint"/> represents.</typeparam>
    /// <typeparam name="TEndpoint">The specific type of <see cref="ICollectionEndpoint{TEntity,TElementEndpoint}"/> to operate on.</typeparam>
    /// <typeparam name="TElementEndpoint">The specific type of <see cref="IElementEndpoint{TEntity}"/> the <typeparamref name="TEndpoint"/> provides for individual <typeparamref name="TEntity"/>s.</typeparam>
    public abstract class PagedCollectionViewModelBase<TEntity, TEndpoint, TElementEndpoint> : CollectionViewModelBase<TEntity, TEndpoint, TElementEndpoint>
        where TEndpoint : class, ICollectionEndpoint<TEntity, TElementEndpoint>
        where TElementEndpoint : class, IEndpoint
    {
        /// <summary>
        /// Creates a new REST paged collection view model.
        /// </summary>
        /// <param name="endpoint">The endpoint this view model operates on.</param>
        /// <param name="eventAggregator">Used to send refresh notifications.</param>
        protected PagedCollectionViewModelBase(TEndpoint endpoint, IEventAggregator eventAggregator)
            : base(endpoint, eventAggregator)
        {}

        protected override async Task OnLoadAsync()
        {
            // TODO
            await Endpoint.ReadRangeAsync(new RangeItemHeaderValue(0, 0), CancellationToken);

            CanCreate = Endpoint.CreateAllowed.GetValueOrDefault(CanCreate);
            NotifyOfPropertyChange(() => CanCreate);
        }
    }
}
