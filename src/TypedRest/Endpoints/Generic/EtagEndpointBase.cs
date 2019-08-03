using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using TypedRest.Http;

namespace TypedRest.Endpoints.Generic
{
    /// <summary>
    /// Base class for building endpoints that use ETags (entity tags) for caching and to avoid lost updates.
    /// </summary>
    public abstract class ETagEndpointBase : EndpointBase, ICachingEndpoint
    {
        /// <summary>
        /// Creates a new endpoint with a relative URI.
        /// </summary>
        /// <param name="referrer">The endpoint used to navigate to this one.</param>
        /// <param name="relativeUri">The URI of this endpoint relative to the <paramref name="referrer"/>'s. Prefix <c>./</c> to append a trailing slash to the <paramref name="referrer"/> URI if missing.</param>
        protected ETagEndpointBase(IEndpoint referrer, Uri relativeUri)
            : base(referrer, relativeUri)
        {}

        /// <summary>
        /// Creates a new endpoint with a relative URI.
        /// </summary>
        /// <param name="referrer">The endpoint used to navigate to this one.</param>
        /// <param name="relativeUri">The URI of this endpoint relative to the <paramref name="referrer"/>'s. Prefix <c>./</c> to append a trailing slash to the <paramref name="referrer"/> URI if missing.</param>
        protected ETagEndpointBase(IEndpoint referrer, string relativeUri)
            : base(referrer, relativeUri)
        {}

        public ResponseCache ResponseCache { get; set; }

        /// <summary>
        /// Performs an HTTP GET request on the <see cref="IEndpoint.Uri"/> and caches the response if the server sends an <see cref="HttpResponseHeaders.ETag"/>.
        /// </summary>
        /// <remarks>Sends If-None-Match header if there is already a cached ETag.</remarks>
        /// <param name="cancellationToken">Used to cancel the request.</param>
        /// <returns>The response of the request or the cached response if the server responded with <see cref="HttpStatusCode.NotModified"/>.</returns>
        /// <exception cref="AuthenticationException"><see cref="HttpStatusCode.Unauthorized"/></exception>
        /// <exception cref="UnauthorizedAccessException"><see cref="HttpStatusCode.Forbidden"/></exception>
        /// <exception cref="KeyNotFoundException"><see cref="HttpStatusCode.NotFound"/> or <see cref="HttpStatusCode.Gone"/></exception>
        /// <exception cref="HttpRequestException">Other non-success status code.</exception>
        protected async Task<HttpContent> GetContentAsync(CancellationToken cancellationToken)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, Uri);
            var cache = ResponseCache; // Copy reference for thread-safety
            if (cache?.ETag != null) request.Headers.IfNoneMatch.Add(cache.ETag);

            var response = await HttpClient.SendAsync(request, cancellationToken).NoContext();
            if (response.StatusCode == HttpStatusCode.NotModified && cache != null)
                return cache.GetContent();
            else
            {
                await HandleResponseAsync(Task.FromResult(response)).NoContext();
                if (response.Content != null) ResponseCache = new ResponseCache(response);
                return response.Content;
            }
        }

        /// <summary>
        /// Performs an HTTP PUT request on the <see cref="IEndpoint.Uri"/>. Sets <see cref="HttpRequestHeaders.IfMatch"/> if there is a cached ETag to detect lost updates.
        /// </summary>
        /// <param name="content">The content to send to the server.</param>
        /// <param name="cancellationToken">Used to cancel the request.</param>
        /// <returns>The response message.</returns>
        /// <exception cref="InvalidDataException"><see cref="HttpStatusCode.BadRequest"/></exception>
        /// <exception cref="AuthenticationException"><see cref="HttpStatusCode.Unauthorized"/></exception>
        /// <exception cref="UnauthorizedAccessException"><see cref="HttpStatusCode.Forbidden"/></exception>
        /// <exception cref="KeyNotFoundException"><see cref="HttpStatusCode.NotFound"/> or <see cref="HttpStatusCode.Gone"/></exception>
        /// <exception cref="InvalidOperationException">The content has changed since it was last retrieved with <see cref="GetContentAsync"/>. Your changes were rejected to prevent a lost update.</exception>
        /// <exception cref="HttpRequestException">Other non-success status code.</exception>
        protected Task<HttpResponseMessage> PutContentAsync(HttpContent content, CancellationToken cancellationToken)
        {
            var request = new HttpRequestMessage(HttpMethod.Put, Uri) {Content = content};
            var cache = ResponseCache; // Copy reference for thread-safety
            if (cache?.ETag != null) request.Headers.IfMatch.Add(cache.ETag);

            ResponseCache = null;
            return HandleResponseAsync(HttpClient.SendAsync(request, cancellationToken));
        }

        /// <summary>
        /// Performs an HTTP DELETE request on the <see cref="IEndpoint.Uri"/>. Sets <see cref="HttpRequestHeaders.IfMatch"/> if there is a cached ETag to detect lost updates.
        /// </summary>
        /// <param name="cancellationToken">Used to cancel the request.</param>
        /// <returns>The response message.</returns>
        /// <exception cref="InvalidDataException"><see cref="HttpStatusCode.BadRequest"/></exception>
        /// <exception cref="AuthenticationException"><see cref="HttpStatusCode.Unauthorized"/></exception>
        /// <exception cref="UnauthorizedAccessException"><see cref="HttpStatusCode.Forbidden"/></exception>
        /// <exception cref="KeyNotFoundException"><see cref="HttpStatusCode.NotFound"/> or <see cref="HttpStatusCode.Gone"/></exception>
        /// <exception cref="InvalidOperationException">The content has changed since it was last retrieved with <see cref="GetContentAsync"/>. Your changes were rejected to prevent a lost update.</exception>
        /// <exception cref="HttpRequestException">Other non-success status code.</exception>
        protected Task<HttpResponseMessage> DeleteContentAsync(CancellationToken cancellationToken)
        {
            var request = new HttpRequestMessage(HttpMethod.Delete, Uri);
            var cache = ResponseCache; // Copy reference for thread-safety
            if (cache?.ETag != null) request.Headers.IfMatch.Add(cache.ETag);

            ResponseCache = null;
            return HandleResponseAsync(HttpClient.SendAsync(request, cancellationToken));
        }
    }
}
