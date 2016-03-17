using Caliburn.Micro;

namespace TypedRest.Wpf.ViewModels
{
    /// <summary>
    /// View model operating on a <see cref="IStreamEndpoint{TEntity}"/>.
    /// </summary>
    /// <typeparam name="TEntity">The type of entity the <see cref="IStreamEndpoint{TEntity}"/> represents.</typeparam>
    public class StreamViewModel<TEntity> : StreamViewModelBase<TEntity, IStreamEndpoint<TEntity>, IElementEndpoint<TEntity>>
    {
        /// <summary>
        /// Creates a new REST paged collection view model.
        /// </summary>
        /// <param name="endpoint">The REST endpoint this view model operates on.</param>
        /// <param name="eventAggregator">Used to send refresh notifications.</param>
        public StreamViewModel(IStreamEndpoint<TEntity> endpoint, IEventAggregator eventAggregator)
            : base(endpoint, eventAggregator)
        {
        }

        protected override IScreen BuildElementScreen(IElementEndpoint<TEntity> elementEndpoint)
        {
            return new ElementViewModel<TEntity>(elementEndpoint, EventAggregator);
        }

        protected override IScreen BuildCreateElementScreen()
        {
            return new CreateElementViewModel<TEntity>(Endpoint, EventAggregator);
        }
    }
}