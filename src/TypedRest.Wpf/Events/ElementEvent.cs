using Caliburn.Micro;

namespace TypedRest.Wpf.Events
{
    /// <summary>
    /// An event for the <see cref="IEventAggregator"/> that references an <see cref="IElementEndpoint{TEntity}"/>.
    /// </summary>
    /// <typeparam name="TEntity">The type of entity the endpoint represents.</typeparam>
    public abstract class ElementEvent<TEntity> : EndpointEvent<IElementEndpoint<TEntity>>
    {
        /// <summary>
        /// Creates a new element event.
        /// </summary>
        /// <param name="endpoint">The endpoint that raised the event.</param>
        protected ElementEvent(IElementEndpoint<TEntity> endpoint)
            : base(endpoint)
        {}
    }
}
