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
    /// <param name="serializers">A list of serializers available for deserializing the body.</param>
    /// <param name="cancellationToken">Used to cancel the request.</param>
    public static Task<T> ReadAsAsync<T>(this HttpContent content, IReadOnlyList<MediaTypeFormatter> serializers, CancellationToken cancellationToken = default)
    {
        HandleCustomJsonMediaTypes(content.Headers, serializers);
        return content.ReadAsAsync<T>(serializers.AsEnumerable(), cancellationToken);
    }

    private static readonly MediaTypeHeaderValue _jsonMediaType = new("application/json");

    private static void HandleCustomJsonMediaTypes(HttpContentHeaders headers, IEnumerable<MediaTypeFormatter> serializers)
    {
        if (headers.ContentType?.MediaType is {} mediaType
         && mediaType.EndsWith("+json")
         && serializers.Any(x => x.SupportedMediaTypes.Contains(_jsonMediaType)))
            headers.ContentType.MediaType = _jsonMediaType.MediaType!;
    }
}
