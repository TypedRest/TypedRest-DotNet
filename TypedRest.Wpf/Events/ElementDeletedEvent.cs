namespace TypedRest.Wpf.Events
{
    /// <summary>
    /// Indicates that an element was deleted.
    /// </summary>
    /// <typeparam name="TEntity">The type of entity that was deleted.</typeparam>
    /// <seealso cref="IElementEndpoint{TEntity}.DeleteAsync"/>
    public class ElementDeletedEvent<TEntity> : ElementEvent<TEntity>
    {
        /// <summary>
        /// Creates a new element delete event.
        /// </summary>
        /// <param name="endpoint">The endpoint representing the deleted entity.</param>
        public ElementDeletedEvent(IElementEndpoint<TEntity> endpoint)
            : base(endpoint)
        {}
    }
}
