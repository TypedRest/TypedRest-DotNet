using Caliburn.Micro;
using TypedRest.Endpoints.Generic;
using TypedRest.Endpoints.Reactive;

namespace TypedRest.Wpf.ViewModels
{
    /// <summary>
    /// View model operating on a <see cref="IStreamingCollectionEndpoint{TEntity}"/>.
    /// </summary>
    /// <typeparam name="TEntity">The type of entity the <see cref="IStreamingCollectionEndpoint{TEntity}"/> represents.</typeparam>
    public class StreamingCollectionViewModel<TEntity> : StreamingCollectionViewModelBase<TEntity, IStreamingCollectionEndpoint<TEntity>, IElementEndpoint<TEntity>>
    {
        /// <summary>
        /// Creates a new REST paged collection view model.
        /// </summary>
        /// <param name="endpoint">The endpoint this view model operates on.</param>
        /// <param name="eventAggregator">Used to send refresh notifications.</param>
        public StreamingCollectionViewModel(IStreamingCollectionEndpoint<TEntity> endpoint, IEventAggregator eventAggregator)
            : base(endpoint, eventAggregator)
        {}

        protected override IScreen BuildElementScreen(IElementEndpoint<TEntity> elementEndpoint)
            => new ElementViewModel<TEntity>(elementEndpoint, EventAggregator);

        protected override IScreen BuildCreateElementScreen()
            => new CreateElementViewModel<TEntity>(Endpoint, EventAggregator);
    }
}
