using System.Threading.Tasks;
using Caliburn.Micro;
using TypedRest.Wpf.Events;

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
        /// <param name="eventAggregator">Used to send refresh notifications.</param>
        public ElementViewModel(IElementEndpoint<TEntity> endpoint, IEventAggregator eventAggregator)
            : base(endpoint, eventAggregator)
        {
        }

        protected override async Task OnLoadAsync()
        {
            Entity = await Endpoint.ReadAsync(CancellationToken);
            DisplayName = Entity.ToString();
            NotifyOfPropertyChange(() => Entity);
        }

        protected override async Task OnSaveAsync()
        {
            await Endpoint.UpdateAsync(Entity, CancellationToken);
            EventAggregator.Publish(new ElementUpdatedEvent<TEntity>(Endpoint), null);
        }
    }
}