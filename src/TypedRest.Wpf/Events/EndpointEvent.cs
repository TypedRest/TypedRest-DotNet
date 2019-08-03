using Caliburn.Micro;
using TypedRest.Endpoints;

namespace TypedRest.Wpf.Events
{
    /// <summary>
    /// An event for the <see cref="IEventAggregator"/> that references an <see cref="IEndpoint"/>.
    /// </summary>
    /// <typeparam name="TEndpoint">The type of endpoint that raised the event.</typeparam>
    public abstract class EndpointEvent<TEndpoint> : IEndpointEvent
        where TEndpoint : class, IEndpoint
    {
        /// <summary>
        /// The endpoint that raised the event.
        /// </summary>
        public TEndpoint Endpoint { get; }

        IEndpoint IEndpointEvent.Endpoint => Endpoint;

        /// <summary>
        /// Creates a new endpoint event.
        /// </summary>
        /// <param name="endpoint">The endpoint that raised the event.</param>
        protected EndpointEvent(TEndpoint endpoint)
        {
            Endpoint = endpoint;
        }
    }
}
