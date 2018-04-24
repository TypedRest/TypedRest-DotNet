using Caliburn.Micro;

namespace TypedRest.Wpf.ViewModels
{
    /// <summary>
    /// View model operating on a <see cref="ICollectionEndpoint{TEntity}"/>.
    /// </summary>
    /// <typeparam name="TEntity">The type of entity the <see cref="ICollectionEndpoint{TEntity}"/> represents.</typeparam>
    public class CollectionViewModel<TEntity> : CollectionViewModelBase<TEntity, ICollectionEndpoint<TEntity>, IElementEndpoint<TEntity>>
    {
        /// <summary>
        /// Creates a new REST collection view model.
        /// </summary>
        /// <param name="endpoint">The REST endpoint this view model operates on.</param>
        /// <param name="eventAggregator">Used to send refresh notifications.</param>
        public CollectionViewModel(ICollectionEndpoint<TEntity> endpoint, IEventAggregator eventAggregator)
            : base(endpoint, eventAggregator)
        {}

        protected override IScreen BuildElementScreen(IElementEndpoint<TEntity> elementEndpoint) => new ElementViewModel<TEntity>(elementEndpoint, EventAggregator);

        protected override IScreen BuildCreateElementScreen() => new CreateElementViewModel<TEntity>(Endpoint, EventAggregator);
    }
}
