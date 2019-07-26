namespace TypedRest
{
    /// <summary>
    /// Endpoint that caches the last response.
    /// </summary>
    public interface ICachingEndpoint : IEndpoint
    {
        /// <summary>
        /// A cached copy of the last response. Can be null.
        /// </summary>
        ResponseCache ResponseCache { get; set; }
    }
}
