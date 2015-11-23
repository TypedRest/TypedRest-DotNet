using Caliburn.Micro;

namespace TypedRest.Wpf.ViewModels
{
    /// <summary>
    /// View model operating on a <see cref="StreamEndpoint{TEntity}"/>.
    /// </summary>
    /// <typeparam name="TEntity">The type of entity the <see cref="StreamEndpoint{TEntity}"/> represents.</typeparam>
    public class StreamViewModel<TEntity> : StreamViewModelBase<TEntity, StreamEndpoint<TEntity>, ElementEndpoint<TEntity>>
    {
        /// <summary>
        /// Creates a new REST paged collection view model.
        /// </summary>
        /// <param name="endpoint">The REST endpoint this view model operates on.</param>
        public StreamViewModel(StreamEndpoint<TEntity> endpoint) : base(endpoint)
        {
        }

        protected override IScreen BuildElementScreen(ElementEndpoint<TEntity> elementEndpoint)
        {
            return new ElementViewModel<TEntity>(elementEndpoint);
        }

        protected override IScreen BuildCreateElementScreen()
        {
            return new CreateElementViewModel<TEntity, ElementEndpoint<TEntity>>(Endpoint);
        }
    }
}