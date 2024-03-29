namespace TypedRest.Http;

/// <summary>
/// Provides additional values for <see cref="HttpMethod"/>.
/// </summary>
public static class HttpMethods
{
    /// <summary>
    /// Represents an HTTP PATCH protocol method that is used to modify an existing entity at a URI.
    /// </summary>
    public static HttpMethod Patch { get; } = new("PATCH");
}