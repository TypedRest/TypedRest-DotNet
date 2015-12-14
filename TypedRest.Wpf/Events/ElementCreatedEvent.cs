namespace TypedRest.Wpf.Events
{
    /// <summary>
    /// Indicates that <see cref="ICollectionEndpoint{TEntity,TElementEndpoint}.CreateAsync"/> was called. Reports the resulting <see cref="IElementEndpoint{TEntity}"/>.
    /// </summary>
    /// <typeparam name="TEntity">The type of entity the endpoint represents.</typeparam>
    public class ElementCreatedEvent<TEntity> : ElementEvent<TEntity>
    {
        /// <summary>
        /// Creates a new element created event.
        /// </summary>
        /// <param name="endpoint">The endpoint representing the newly created entity.</param>
        public ElementCreatedEvent(IElementEndpoint<TEntity> endpoint) : base(endpoint)
        {
        }
    }
}