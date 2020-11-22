using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading;
using System.Threading.Tasks;

namespace TypedRest.Http
{
    /// <summary>
    /// Provides extension methods for <see cref="HttpContent"/>.
    /// </summary>
    public static class HttpContentExtensions
    {
        /// <summary>
        /// Deserializes HTTP content as an object.
        /// </summary>
        /// <typeparam name="T">The type of the object to read.</typeparam>
        /// <param name="content">The HTTP content from which to read.</param>
        /// <param name="serializer">The serializer to use for deserialization.</param>
        /// <param name="cancellationToken">Used to cancel the request.</param>
        public static Task<T> ReadAsAsync<T>(this HttpContent content, MediaTypeFormatter serializer, CancellationToken cancellationToken = default)
        {
            if (content.Headers.ContentType?.MediaType != null
             && content.Headers.ContentType.MediaType.EndsWith("+json")
             && serializer is JsonMediaTypeFormatter)
                content.Headers.ContentType.MediaType = "application/json";

            return content.ReadAsAsync<T>(new[] {serializer}, cancellationToken);
        }
    }
}
