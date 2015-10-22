using Caliburn.Micro;

namespace TypedRest.Wpf.ViewModels
{
    /// <summary>
    /// View model operating on a <see cref="CollectionEndpoint{TEntity}"/>.
    /// </summary>
    /// <typeparam name="TEntity">The type of entity the <see cref="CollectionEndpoint{TEntity}"/> represents.</typeparam>
    public class CollectionViewModel<TEntity> : CollectionViewModelBase<TEntity, CollectionEndpoint<TEntity>, ElementEndpoint<TEntity>>
    {
        /// <summary>
        /// Creates a new REST collection view model.
        /// </summary>
        /// <param name="endpoint">The REST endpoint this view model operates on.</param>
        public CollectionViewModel(CollectionEndpoint<TEntity> endpoint) : base(endpoint)
        {
        }

        protected override IScreen GetElementScreen(ElementEndpoint<TEntity> elementEndpoint)
        {
            return new ElementViewModel<TEntity>(elementEndpoint);
        }
    }
}