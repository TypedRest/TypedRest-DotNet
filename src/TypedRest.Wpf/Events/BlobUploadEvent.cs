using TypedRest.Endpoints.Raw;

namespace TypedRest.Wpf.Events
{
    /// <summary>
    /// Indicates that <see cref="IBlobEndpoint.UploadFromAsync"/> was called.
    /// </summary>
    public class BlobUploadEvent : EndpointEvent<IBlobEndpoint>
    {
        /// <summary>
        /// Creates a new blob upload event.
        /// </summary>
        /// <param name="endpoint">The endpoint that data was uploaded to.</param>
        public BlobUploadEvent(IBlobEndpoint endpoint)
            : base(endpoint)
        {}
    }
}
