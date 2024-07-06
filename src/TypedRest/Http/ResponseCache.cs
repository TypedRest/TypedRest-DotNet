namespace TypedRest.Http;

/// <summary>
/// Captures the content of an <see cref="HttpResponseMessage"/> for caching.
/// </summary>
public sealed class ResponseCache
{
    private readonly byte[] _content;
    private readonly MediaTypeHeaderValue? _contentType;
    private readonly EntityTagHeaderValue? _eTag;
    private readonly DateTimeOffset? _lastModified;
    private readonly DateTimeOffset? _expires;

    /// <summary>
    /// Creates a <see cref="ResponseCache"/> from a <paramref name="response"/> if it is eligible for caching.
    /// </summary>
    /// <returns>The <see cref="ResponseCache"/>; <c>null</c> if the response is not eligible for caching.</returns>
    public static ResponseCache? From(HttpResponseMessage response)
        => response is {IsSuccessStatusCode: true, Headers.CacheControl: null or {NoStore: false}}
            ? new(response)
            : null;

    private ResponseCache(HttpResponseMessage response)
    {
        _content = ReadContent(response.Content ?? throw new ArgumentException("Missing content.", nameof(response)));

        _contentType = response.Content.Headers.ContentType;
        _eTag = response.Headers.ETag;
        _lastModified = response.Content.Headers.LastModified;

        _expires = response.Content.Headers.Expires;
        if (_expires == null && response.Headers.CacheControl?.MaxAge is {} maxAge)
            _expires = DateTimeOffset.Now + maxAge;

        // Treat no-cache as expired immediately
        if (response.Headers.CacheControl?.NoCache ?? false)
            _expires = DateTimeOffset.Now;
    }

    private static byte[] ReadContent(HttpContent content)
    {
        var result = content.ReadAsByteArrayAsync().Result;

        // Rewind stream if possible
        var stream = content.ReadAsStreamAsync().Result;
        if (stream.CanSeek) stream.Position = 0;

        return result;
    }

    /// <summary>
    /// Indicates whether this cached response has expired.
    /// </summary>
    public bool IsExpired
        => _expires.HasValue && DateTime.Now >= _expires;

    /// <summary>
    /// Returns the cached <see cref="HttpClient"/>.
    /// </summary>
    public HttpContent GetContent()
    {
        // Build new response for each request to avoid shared Stream.Position
        return new ByteArrayContent(_content) {Headers = {ContentType = _contentType}};
    }

    /// <summary>
    /// Sets request headers that require that the resource has been modified since it was cached.
    /// </summary>
    public void SetIfModifiedHeaders(HttpRequestHeaders headers)
    {
        if (_eTag != null) headers.IfNoneMatch.Add(_eTag);
        else if (_lastModified != null) headers.IfModifiedSince = _lastModified;
    }

    /// <summary>
    /// Sets request headers that require that the resource has not been modified since it was cached.
    /// </summary>
    public void SetIfUnmodifiedHeaders(HttpRequestHeaders headers)
    {
        if (_eTag != null) headers.IfMatch.Add(_eTag);
        else if (_lastModified != null) headers.IfUnmodifiedSince = _lastModified;
    }
}
