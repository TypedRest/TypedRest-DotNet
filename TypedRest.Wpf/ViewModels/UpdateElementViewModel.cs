using System.Threading.Tasks;

namespace TypedRest.Wpf.ViewModels
{
    /// <summary>
    /// View model for updating an existing <see cref="IElementEndpoint{TEntity}"/>.
    /// </summary>
    /// <typeparam name="TEntity">The type of entity to update.</typeparam>
    public class UpdateElementViewModel<TEntity> : ElementViewModelBase<TEntity, IElementEndpoint<TEntity>>
    {
        /// <summary>
        /// Creates a new REST element updating view model.
        /// </summary>
        /// <param name="endpoint">The REST endpoint this view model operates on.</param>
        public UpdateElementViewModel(IElementEndpoint<TEntity> endpoint) : base(endpoint)
        {
        }

        protected override async Task OnLoad()
        {
            Entity = await Endpoint.ReadAsync(CancellationToken);
            DisplayName = Entity.ToString();
            NotifyOfPropertyChange(() => Entity);
        }

        protected override async Task OnSave()
        {
            await Endpoint.UpdateAsync(Entity, CancellationToken);
        }
    }
}