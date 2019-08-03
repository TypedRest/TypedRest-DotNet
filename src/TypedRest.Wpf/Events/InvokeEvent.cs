using TypedRest.Endpoints.Rpc;

namespace TypedRest.Wpf.Events
{
    /// <summary>
    /// Indicates that <see cref="IRpcEndpoint"/>.InvokeAsync() was called.
    /// </summary>
    public class InvokeEvent : EndpointEvent<IRpcEndpoint>
    {
        /// <summary>
        /// Creates a new invoke event.
        /// </summary>
        /// <param name="endpoint">The endpoint that was invoked.</param>
        public InvokeEvent(IRpcEndpoint endpoint)
            : base(endpoint)
        {}
    }
}
