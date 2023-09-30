namespace TypedRest.Http;

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
        HandleCustomJsonMediaTypes(content.Headers, serializer);
        return content.ReadAsAsync<T>(new[] {serializer}, cancellationToken);
    }

    private static readonly MediaTypeHeaderValue _jsonMediaType = new("application/json");

    private static void HandleCustomJsonMediaTypes(HttpContentHeaders headers, MediaTypeFormatter serializer)
    {
        if (headers.ContentType?.MediaType is {} mediaType
         && mediaType.EndsWith("+json")
         && serializer.SupportedMediaTypes.Contains(_jsonMediaType))
            headers.ContentType.MediaType = _jsonMediaType.MediaType!;
    }
}
