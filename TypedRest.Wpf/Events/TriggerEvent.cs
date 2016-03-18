namespace TypedRest.Wpf.Events
{
    /// <summary>
    /// Indicates that <see cref="IActionEndpoint.TriggerAsync"/> was called.
    /// </summary>
    public class TriggerEvent : EndpointEvent<IActionEndpoint>
    {
        /// <summary>
        /// Creates a new trigger event.
        /// </summary>
        /// <param name="endpoint">The endpoint that was triggered.</param>
        public TriggerEvent(IActionEndpoint endpoint) : base(endpoint)
        {
        }
    }
}