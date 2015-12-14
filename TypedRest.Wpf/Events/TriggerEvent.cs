namespace TypedRest.Wpf.Events
{
    /// <summary>
    /// Indicates that <see cref="ITriggerEndpoint.TriggerAsync"/> was called.
    /// </summary>
    /// <typeparam name="TEndpoint">The type of endpoint that raised the event.</typeparam>
    public class TriggerEvent<TEndpoint> : EndpointEvent<TEndpoint>
        where TEndpoint : ITriggerEndpoint
    {
        /// <summary>
        /// Creates a new trigger event.
        /// </summary>
        /// <param name="endpoint">The endpoint that was triggered.</param>
        public TriggerEvent(TEndpoint endpoint) : base(endpoint)
        {
        }
    }
}