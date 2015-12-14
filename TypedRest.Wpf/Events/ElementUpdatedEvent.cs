namespace TypedRest.Wpf.Events
{
    /// <summary>
    /// Indicates that <see cref="IElementEndpoint{TEntity}.UpdateAsync"/> was called.
    /// </summary>
    /// <typeparam name="TEntity">The type of entity the endpoint represents.</typeparam>
    public class ElementUpdatedEvent<TEntity> : ElementEvent<TEntity>
    {
        /// <summary>
        /// Creates a new element updated event.
        /// </summary>
        /// <param name="endpoint">The endpoint representing the newly updated entity.</param>
        public ElementUpdatedEvent(IElementEndpoint<TEntity> endpoint) : base(endpoint)
        {
        }
    }
}