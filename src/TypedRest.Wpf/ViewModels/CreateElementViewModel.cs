using System.Threading.Tasks;
using Caliburn.Micro;
using TypedRest.Endpoints.Generic;
using TypedRest.Wpf.Events;

namespace TypedRest.Wpf.ViewModels
{
    /// <summary>
    /// View model for creating a new element in a <see cref="ICollectionEndpoint{TEntity,TElementEndpoint}"/>.
    /// </summary>
    /// <remarks>Use the more constrained <see cref="CreateElementViewModel{TEntity}"/> when possible.</remarks>
    /// <typeparam name="TEntity">The type of entity to create.</typeparam>
    /// <typeparam name="TElementEndpoint">The specific type of <see cref="IElementEndpoint{TEntity}"/> the <see cref="ICollectionEndpoint{TEntity,TElementEndpoint}"/> provides for <typeparamref name="TEntity"/>s.</typeparam>
    public class CreateElementViewModel<TEntity, TElementEndpoint> : ElementViewModelBase<TEntity, ICollectionEndpoint<TEntity, TElementEndpoint>>
        where TElementEndpoint : class, IElementEndpoint<TEntity>
    {
        /// <summary>
        /// Creates a new REST element creation view model.
        /// </summary>
        /// <param name="endpoint">The endpoint this view model operates on.</param>
        /// <param name="eventAggregator">Used to send refresh notifications.</param>
        public CreateElementViewModel(ICollectionEndpoint<TEntity, TElementEndpoint> endpoint, IEventAggregator eventAggregator)
            : base(endpoint, eventAggregator)
        {
            DisplayName = "New " + typeof(TEntity).Name;
        }

        protected override async Task OnSaveAsync()
        {
            var newEndpoint = await Endpoint.CreateAsync(Entity, CancellationToken);
            EventAggregator.Publish(new ElementCreatedEvent<TEntity>(newEndpoint), null);
        }
    }

    /// <summary>
    /// View model for creating a new element in a <see cref="ICollectionEndpoint{TEntity}"/>.
    /// </summary>
    /// <typeparam name="TEntity">The type of entity to create.</typeparam>
    public class CreateElementViewModel<TEntity> : CreateElementViewModel<TEntity, IElementEndpoint<TEntity>>
    {
        /// <summary>
        /// Creates a new REST element creation view model.
        /// </summary>
        /// <param name="endpoint">The endpoint this view model operates on.</param>
        /// <param name="eventAggregator">Used to send refresh notifications.</param>
        public CreateElementViewModel(ICollectionEndpoint<TEntity, IElementEndpoint<TEntity>> endpoint, IEventAggregator eventAggregator)
            : base(endpoint, eventAggregator)
        {}
    }
}
