using System.Runtime.CompilerServices;

namespace TypedRest.Endpoints.Generic;

/// <summary>
/// Base class for building endpoints that use ETags and Last-Modified timestamps for caching and to avoid lost updates.
/// </summary>
/// <param name="referrer">The endpoint used to navigate to this one.</param>
/// <param name="relativeUri">The URI of this endpoint relative to the <paramref name="referrer"/>'s. Add a <c>./</c> prefix here to imply a trailing slash in <paramref name="referrer"/>'s URI.</param>
public abstract class CachingEndpointBase(IEndpoint referrer, Uri relativeUri)
    : EndpointBase(referrer, relativeUri), ICachingEndpoint
{
    /// <summary>
    /// Creates a new endpoint with a relative URI.
    /// </summary>
    /// <param name="referrer">The endpoint used to navigate to this one.</param>
    /// <param name="relativeUri">The URI of this endpoint relative to the <paramref name="referrer"/>'s. Add a <c>./</c> prefix here to imply a trailing slash in <paramref name="referrer"/>'s URI.</param>
    protected CachingEndpointBase(IEndpoint referrer, string relativeUri)
        : this(referrer, new Uri(relativeUri, UriKind.Relative)) {}

    public ResponseCache? ResponseCache { get; set; }

    // Track writes, so responses to requests that overlap a write do not cache outdated content.
    private int _writesInProgress, _finishedWrites;
    private readonly object _cacheLock = new();

    private int StartWrite()
    {
        lock (_cacheLock)
        {
            ResponseCache = null;
            _writesInProgress++;
            return _finishedWrites;
        }
    }

    private void EndWrite(int finishedWrites, ResponseCache? cache = null)
    {
        lock (_cacheLock)
        {
            _writesInProgress--;
            if (cache != null && _writesInProgress == 0 && _finishedWrites == finishedWrites) ResponseCache = cache;
            _finishedWrites++;
        }
    }

    private (ResponseCache? cache, int finishedWrites) GetResponseCache()
    {
        lock (_cacheLock)
        {
            return (ResponseCache, _finishedWrites);
        }
    }

    private void StoreResponseCache(ResponseCache? cache, int finishedWrites)
    {
        lock (_cacheLock)
        {
            if (_writesInProgress == 0 && _finishedWrites == finishedWrites)
                ResponseCache = cache;
        }
    }

    /// <summary>
    /// Sends a request that modifies the resource and discards <see cref="ResponseCache"/>.
    /// </summary>
    /// <param name="request">Sends the request.</param>
    /// <returns>The result of <paramref name="request"/>.</returns>
    protected async Task<T> TrackWriteAsync<T>(Func<Task<T>> request)
    {
        int finishedWrites = StartWrite();
        try
        {
            return await request().NoContext();
        }
        finally
        {
            EndWrite(finishedWrites);
        }
    }

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
        using var request = new HttpRequestMessage(HttpMethod.Get, Uri);

        (var cache, int finishedWrites) = GetResponseCache();
        cache?.SetIfModifiedHeaders(request.Headers); // Only fetch if changed

        var response = await HttpClient.SendAsync(request, cancellationToken).NoContext();
        if (response.StatusCode == HttpStatusCode.NotModified && cache != null)
        {
            response.Dispose();
            return cache.GetContent();
        }
        else
        {
            response = await HandleAsync(() => Task.FromResult(response), caller);
            StoreResponseCache(ResponseCache.From(response), finishedWrites);
            return response.Content;
        }
    }

    /// <summary>
    /// Performs an <see cref="HttpMethod.Put"/> request on the <see cref="IEndpoint.Uri"/>.
    /// </summary>
    /// <remarks>
    /// Sends <see cref="HttpRequestHeaders.IfMatch"/> if there is a cached strong <see cref="HttpResponseHeader.ETag"/> to detect lost updates.
    /// Caches the response if the server sends an <see cref="HttpResponseHeaders.ETag"/>, so later requests can be conditional without reading the resource again.
    /// </remarks>
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
        using var request = new HttpRequestMessage(HttpMethod.Put, Uri) {Content = content};
        ResponseCache?.SetIfUnmodifiedHeaders(request.Headers); // Prevent lost updates

        int finishedWrites = StartWrite();
        ResponseCache? cache = null;
        try
        {
            // Buffer the request body before sending, so it is still available for caching afterwards
            var requestContent = await content.ReadAsByteArrayAsync().NoContext();

            var response = await HandleAsync(() => HttpClient.SendAsync(request, cancellationToken), caller).NoContext();
            cache = ResponseCache.FromPut(response, requestContent, content.Headers.ContentType);
            return response;
        }
        finally
        {
            EndWrite(finishedWrites, cache);
        }
    }

    /// <summary>
    /// Performs an <see cref="HttpMethod.Delete"/> request on the <see cref="IEndpoint.Uri"/>.
    /// </summary>
    /// <remarks>Sends <see cref="HttpRequestHeaders.IfMatch"/> if there is a cached strong ETag to detect lost updates.</remarks>
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
        using var request = new HttpRequestMessage(HttpMethod.Delete, Uri);
        ResponseCache?.SetIfUnmodifiedHeaders(request.Headers);

        int finishedWrites = StartWrite();
        try
        {
            await FinalizeAsync(() => HttpClient.SendAsync(request, cancellationToken), caller).NoContext();
        }
        finally
        {
            EndWrite(finishedWrites);
        }
    }
}
