using Caliburn.Micro;

namespace TypedRest.Wpf.ViewModels
{
    /// <summary>
    /// View model operating on a <see cref="PagedCollectionEndpoint{TEntity}"/>.
    /// </summary>
    /// <typeparam name="TEntity">The type of entity the <see cref="PagedCollectionEndpoint{TEntity}"/> represents.</typeparam>
    public class PagedCollectionViewModel<TEntity> : PagedCollectionViewModelBase<TEntity, PagedCollectionEndpoint<TEntity>, ElementEndpoint<TEntity>>
    {
        /// <summary>
        /// Creates a new REST paged collection view model.
        /// </summary>
        /// <param name="endpoint">The REST endpoint this view model operates on.</param>
        public PagedCollectionViewModel(PagedCollectionEndpoint<TEntity> endpoint) : base(endpoint)
        {
        }

        protected override IScreen BuildCreateElementScreen()
        {
            return new CreateElementViewModel<TEntity, ElementEndpoint<TEntity>>(Endpoint);
        }

        protected override IScreen BuildUpdateElementScreen(ElementEndpoint<TEntity> elementEndpoint)
        {
            return new UpdateElementViewModel<TEntity>(elementEndpoint);
        }
    }
}