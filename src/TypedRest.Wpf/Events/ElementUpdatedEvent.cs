using TypedRest.Endpoints.Generic;

namespace TypedRest.Wpf.Events
{
    /// <summary>
    /// Indicates that an existing element was updated.
    /// </summary>
    /// <typeparam name="TEntity">The type of entity that was updated.</typeparam>
    /// <seealso cref="IElementEndpoint{TEntity}.SetAsync"/>
    public class ElementUpdatedEvent<TEntity> : ElementEvent<TEntity>
    {
        /// <summary>
        /// Creates a new element update event.
        /// </summary>
        /// <param name="endpoint">The endpoint representing the updated entity.</param>
        public ElementUpdatedEvent(IElementEndpoint<TEntity> endpoint)
            : base(endpoint)
        {}
    }
}
