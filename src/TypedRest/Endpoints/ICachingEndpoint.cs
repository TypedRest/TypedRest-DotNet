using TypedRest.Http;

namespace TypedRest.Endpoints
{
    /// <summary>
    /// Endpoint that caches the last response.
    /// </summary>
    public interface ICachingEndpoint : IEndpoint
    {
        /// <summary>
        /// A cached copy of the last response.
        /// </summary>
        ResponseCache? ResponseCache { get; set; }
    }
}
