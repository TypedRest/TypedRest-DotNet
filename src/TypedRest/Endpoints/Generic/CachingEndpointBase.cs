using System.Runtime.CompilerServices;

namespace TypedRest.Endpoints.Generic;

/// <summary>
/// Base class for building endpoints that use ETags and Last-Modified timestamps for caching and to avoid lost updates.
/// </summary>
/// <param name="referrer">The endpoint used to navigate to this one.</param>
/// <param name="relativeUri">The URI of this endpoint relative to the <paramref name="referrer"/>'s. Add a <c>./</c> prefix here to imply a trailing slash <paramref name="referrer"/>'s URI.</param>
public abstract class CachingEndpointBase(IEndpoint referrer, Uri relativeUri)
    : EndpointBase(referrer, relativeUri), ICachingEndpoint
{
    /// <summary>
    /// Creates a new endpoint with a relative URI.
    /// </summary>
    /// <param name="referrer">The endpoint used to navigate to this one.</param>
    /// <param name="relativeUri">The URI of this endpoint relative to the <paramref name="referrer"/>'s. Add a <c>./</c> prefix here to imply a trailing slash <paramref name="referrer"/>'s URI.</param>
    protected CachingEndpointBase(IEndpoint referrer, string relativeUri)
        : this(referrer, new Uri(relativeUri, UriKind.Relative)) {}

    public ResponseCache? ResponseCache { get; set; }

    /// <summary>
    /// Performs an HTTP GET request on the <see cref="IEndpoint.Uri"/> and caches the response if the server sends an <see cref="HttpResponseHeaders.ETag"/>.
    /// </summary>
    /// <remarks>Sends <see cref="HttpRequestHeaders.IfNoneMatch"/> if there is already a cached ETag.</remarks>
    /// <param name="cancellationToken">Used to cancel the request.</param>
    /// <param name="caller">The name of the method calling this method.</param>
    /// <returns>The response of the request or the cached response if the server responded with <see cref="HttpStatusCode.NotModified"/>.</returns>
    /// <exception cref="AuthenticationException"><see cref="HttpStatusCode.Unauthorized"/></exception>
    /// <exception cref="UnauthorizedAccessException"><see cref="HttpStatusCode.Forbidden"/></exception>
    /// <exception cref="KeyNotFoundException"><see cref="HttpStatusCode.NotFound"/> or <see cref="HttpStatusCode.Gone"/> or empty response body</exception>
    /// <exception cref="HttpRequestException">Other non-success status code.</exception>
    protected async Task<HttpContent> GetContentAsync(CancellationToken cancellationToken, [CallerMemberName] string caller = "unknown")
    {
        var request = new HttpRequestMessage(HttpMethod.Get, Uri);
        var cache = ResponseCache; // Copy reference for thread-safety
        cache?.SetIfModifiedHeaders(request.Headers); // Only fetch if changed

        var response = await HttpClient.SendAsync(request, cancellationToken).NoContext();
        if (response.StatusCode == HttpStatusCode.NotModified && cache is {IsExpired: false})
            return cache.GetContent();
        else
        {
            response = await HandleAsync(() => Task.FromResult(response), caller);
            ResponseCache = ResponseCache.From(response);
            return response.Content;
        }
    }

    /// <summary>
    /// Performs an <see cref="HttpMethod.Put"/> request on the <see cref="IEndpoint.Uri"/>.
    /// </summary>
    /// <remarks>Sends <see cref="HttpRequestHeaders.IfMatch"/> if there is a cached <see cref="HttpResponseHeader.ETag"/> to detect lost updates.</remarks>
    /// <param name="content">The content to send to the server.</param>
    /// <param name="cancellationToken">Used to cancel the request.</param>
    /// <param name="caller">The name of the method calling this method.</param>
    /// <returns>The response message.</returns>
    /// <exception cref="InvalidOperationException">The content has changed since it was last retrieved with <see cref="GetContentAsync"/>. Your changes were rejected to prevent a lost update.</exception>
    /// <exception cref="InvalidDataException"><see cref="HttpStatusCode.BadRequest"/></exception>
    /// <exception cref="AuthenticationException"><see cref="HttpStatusCode.Unauthorized"/></exception>
    /// <exception cref="UnauthorizedAccessException"><see cref="HttpStatusCode.Forbidden"/></exception>
    /// <exception cref="KeyNotFoundException"><see cref="HttpStatusCode.NotFound"/> or <see cref="HttpStatusCode.Gone"/></exception>
    /// <exception cref="HttpRequestException">Other non-success status code.</exception>
    protected async Task<HttpResponseMessage> PutContentAsync(HttpContent content, CancellationToken cancellationToken, [CallerMemberName] string caller = "unknown")
    {
        var request = new HttpRequestMessage(HttpMethod.Put, Uri) {Content = content};
        ResponseCache?.SetIfUnmodifiedHeaders(request.Headers); // Prevent lost updates

        ResponseCache = null;
        return await HandleAsync(() => HttpClient.SendAsync(request, cancellationToken), caller).NoContext();
    }

    /// <summary>
    /// Performs an <see cref="HttpMethod.Delete"/> request on the <see cref="IEndpoint.Uri"/>.
    /// </summary>
    /// <remarks>Sends <see cref="HttpRequestHeaders.IfMatch"/> if there is a cached ETag to detect lost updates.</remarks>
    /// <param name="cancellationToken">Used to cancel the request.</param>
    /// <param name="caller">The name of the method calling this method.</param>
    /// <exception cref="InvalidOperationException">The content has changed since it was last retrieved with <see cref="GetContentAsync"/>. Your changes were rejected to prevent a lost update.</exception>
    /// <exception cref="InvalidDataException"><see cref="HttpStatusCode.BadRequest"/></exception>
    /// <exception cref="AuthenticationException"><see cref="HttpStatusCode.Unauthorized"/></exception>
    /// <exception cref="UnauthorizedAccessException"><see cref="HttpStatusCode.Forbidden"/></exception>
    /// <exception cref="KeyNotFoundException"><see cref="HttpStatusCode.NotFound"/> or <see cref="HttpStatusCode.Gone"/></exception>
    /// <exception cref="HttpRequestException">Other non-success status code.</exception>
    protected async Task DeleteContentAsync(CancellationToken cancellationToken, [CallerMemberName] string caller = "unknown")
    {
        var request = new HttpRequestMessage(HttpMethod.Delete, Uri);
        ResponseCache?.SetIfUnmodifiedHeaders(request.Headers);

        ResponseCache = null;
        await FinalizeAsync(() => HttpClient.SendAsync(request, cancellationToken), caller).NoContext();
    }
}
