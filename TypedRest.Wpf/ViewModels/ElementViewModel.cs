using System.Threading.Tasks;

namespace TypedRest.Wpf.ViewModels
{
    /// <summary>
    /// View model for showing or updating an existing elemented represented by a <see cref="IElementEndpoint{TEntity}"/>.
    /// </summary>
    /// <typeparam name="TEntity">The type of entity to represent.</typeparam>
    public class ElementViewModel<TEntity> : ElementViewModelBase<TEntity, IElementEndpoint<TEntity>>
    {
        /// <summary>
        /// Creates a new REST element view model.
        /// </summary>
        /// <param name="endpoint">The REST endpoint this view model operates on.</param>
        public ElementViewModel(IElementEndpoint<TEntity> endpoint) : base(endpoint)
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