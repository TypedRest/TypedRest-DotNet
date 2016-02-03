namespace TypedRest.Wpf.Events
{
    /// <summary>
    /// Indicates that <see cref="ITriggerEndpoint.TriggerAsync"/> was called.
    /// </summary>
    public class TriggerEvent : EndpointEvent<ITriggerEndpoint>
    {
        /// <summary>
        /// Creates a new trigger event.
        /// </summary>
        /// <param name="endpoint">The endpoint that was triggered.</param>
        public TriggerEvent(ITriggerEndpoint endpoint) : base(endpoint)
        {
        }
    }
}