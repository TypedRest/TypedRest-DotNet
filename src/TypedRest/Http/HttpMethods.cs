using System.Net.Http;

namespace TypedRest.Http
{
    /// <summary>
    /// Provides additional values for <see cref="HttpMethod"/>.
    /// </summary>
    public class HttpMethods
    {
        /// <summary>
        /// Represents an HTTP PATCH protocol method that is used to modify an existing entity at a URI.
        /// </summary>
        public static HttpMethod Patch { get; } = new HttpMethod("PATCH");
    }
}
