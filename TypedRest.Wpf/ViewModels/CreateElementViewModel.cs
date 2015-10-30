using System.Threading.Tasks;

namespace TypedRest.Wpf.ViewModels
{
    /// <summary>
    /// View model for creating a new <see cref="IElementEndpoint{TEntity}"/>.
    /// </summary>
    /// <typeparam name="TEntity">The type of entity to create.</typeparam>
    /// <typeparam name="TElementEndpoint">The specific type of <see cref="IElementEndpoint{TEntity}"/> the <see cref="ICollectionEndpoint{TEntity,TElementEndpoint}"/> provides for <typeparamref name="TEntity"/>s.</typeparam>
    public class CreateElementViewModel<TEntity, TElementEndpoint> : ElementViewModelBase<TEntity, ICollectionEndpoint<TEntity, TElementEndpoint>>
        where TElementEndpoint : class, IElementEndpoint<TEntity>
    {
        /// <summary>
        /// Creates a new REST element creation view model.
        /// </summary>
        /// <param name="endpoint">The REST endpoint this view model operates on.</param>
        public CreateElementViewModel(ICollectionEndpoint<TEntity, TElementEndpoint> endpoint) : base(endpoint)
        {
            DisplayName = "New " + typeof(TEntity).Name;
        }

        protected override async Task OnSave()
        {
            await Endpoint.CreateAsync(Entity, CancellationToken);
        }
    }
}