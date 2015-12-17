using Caliburn.Micro;

namespace TypedRest.Wpf.Events
{
    /// <summary>
    /// An event for the <see cref="IEventAggregator"/> that references an <see cref="IEndpoint"/>.
    /// </summary>
    public interface IEndpointEvent
    {
        /// <summary>
        /// The endpoint that raised the event.
        /// </summary>
        IEndpoint Endpoint { get; }
    }
}