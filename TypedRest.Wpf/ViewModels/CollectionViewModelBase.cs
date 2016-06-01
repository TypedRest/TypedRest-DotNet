using System.Collections.Generic;
using System.Threading.Tasks;
using Caliburn.Micro;
using TypedRest.Wpf.Events;

namespace TypedRest.Wpf.ViewModels
{
    /// <summary>
    /// Base class for building view models operating on an <see cref="ICollectionEndpoint{TEntity,TElementEndpoint}"/>.
    /// </summary>
    /// <typeparam name="TEntity">The type of entity the <typeparamref name="TEndpoint"/> represents.</typeparam>
    /// <typeparam name="TEndpoint">The specific type of <see cref="ICollectionEndpoint{TEntity,TElementEndpoint}"/> to operate on.</typeparam>
    /// <typeparam name="TElementEndpoint">The specific type of <see cref="IElementEndpoint{TEntity}"/> the <typeparamref name="TEndpoint"/> provides for individual <typeparamref name="TEntity"/>s.</typeparam>
    public abstract class CollectionViewModelBase<TEntity, TEndpoint, TElementEndpoint> : EndpointViewModelBase<TEndpoint>, IHandleWithTask<ElementEvent<TEntity>>
        where TEndpoint : ICollectionEndpoint<TEntity, TElementEndpoint>
        where TElementEndpoint : class, IElementEndpoint<TEntity>
    {
        /// <summary>
        /// Creates a new REST collection view model.
        /// </summary>
        /// <param name="endpoint">The REST endpoint this view model operates on.</param>
        /// <param name="eventAggregator">Used to send refresh notifications.</param>
        protected CollectionViewModelBase(TEndpoint endpoint, IEventAggregator eventAggregator)
            : base(endpoint, eventAggregator)
        {
            DisplayName = typeof (TEntity).Name;
        }

        public List<TEntity> Elements { get; private set; }

        public List<TEntity> SelectedElements { get; set; }

        protected override async Task OnLoadAsync()
        {
            Elements = await Endpoint.ReadAllAsync(CancellationToken);
            NotifyOfPropertyChange(() => Elements);

            CanCreate = Endpoint.CreateAllowed.GetValueOrDefault(CanCreate);
            NotifyOfPropertyChange(() => CanCreate);
        }

        /// <summary>
        /// Controls whether selecting individual elements opens an edit view.
        /// </summary>
        public bool CanOpenElement { get; set; }

        /// <summary>
        /// Handler for opening an existing element in the collection.
        /// </summary>
        protected virtual void OnOpenElement(TEntity entity)
        {
            Open(BuildElementScreen(Endpoint[entity]));
        }

        /// <summary>
        /// Builds a sub-<see cref="IScreen"/> for viewing or editing an existing <typeparamref name="TEntity"/> represented by the given <paramref name="elementEndpoint"/>.
        /// </summary>
        protected abstract IScreen BuildElementScreen(TElementEndpoint elementEndpoint);

        /// <summary>
        /// Controls whether a create button is shown.
        /// </summary>
        public bool CanCreate { get; set; }

        /// <summary>
        /// Opens a view for creating a new element in the collection.
        /// </summary>
        public virtual void Create()
        {
            Open(BuildCreateElementScreen());
        }

        /// <summary>
        /// Builds a sub-<see cref="IScreen"/> for creating a new <typeparamref name="TEntity"/> in the collection endpoint.
        /// </summary>
        protected abstract IScreen BuildCreateElementScreen();

        // Refresh when child elements are created or updated
        public async Task Handle(ElementEvent<TEntity> message)
        {
            await RefreshAsync();
        }
    }
}