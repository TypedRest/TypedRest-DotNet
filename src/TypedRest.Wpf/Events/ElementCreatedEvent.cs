using System.Threading;

namespace TypedRest.Wpf.Events
{
    /// <summary>
    /// Indicates that a new element was created.
    /// </summary>
    /// <typeparam name="TEntity">The type of entity that was created.</typeparam>
    /// <seealso cref="ICollectionEndpoint{TEntity,TElementEndpoint}.CreateAsync(TEntity,CancellationToken)"/>
    public class ElementCreatedEvent<TEntity> : ElementEvent<TEntity>
    {
        /// <summary>
        /// Creates a new element create event.
        /// </summary>
        /// <param name="endpoint">The endpoint representing the newly created entity.</param>
        public ElementCreatedEvent(IElementEndpoint<TEntity> endpoint)
            : base(endpoint)
        {}
    }
}
