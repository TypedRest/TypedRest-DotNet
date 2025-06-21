namespace TypedRest.Http;

/// <summary>
/// Provides extension methods for <see cref="HttpResponseHeaders"/>.
/// </summary>
public static class HttpResponseHeadersExtensions
{
    /// <summary>
    /// Returns the relative waiting time extracted from the <c>Retry-After</c> header if any.
    /// </summary>
    public static TimeSpan? RetryAfterDuration(this HttpResponseHeaders headers)
        => headers.RetryAfter?.Delta
        ?? headers.RetryAfter?.Date - DateTimeOffset.UtcNow;
}
