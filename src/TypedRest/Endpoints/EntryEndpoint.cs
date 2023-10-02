using TypedRest.Errors;
using TypedRest.Links;
using TypedRest.Serializers;

namespace TypedRest.Endpoints;

/// <summary>
/// Represent the top-level URI of an API. Derive from this class and add your own set of child-<see cref="IEndpoint"/>s as properties.
/// </summary>
public class EntryEndpoint : EndpointBase
{
    /// <summary>
    /// Creates a new entry endpoint.
    /// </summary>
    /// <param name="httpClient">The HTTP client used to communicate with the REST API.</param>
    /// <param name="uri">The base URI of the REST API. Missing trailing slash will be appended automatically. <see cref="HttpClient.BaseAddress"/> is used if this is unset.</param>
    /// <param name="serializers">A list of serializers used for entities received from the server, sorted from most to least preferred. Always uses first for sending to the server.</param>
    /// <param name="errorHandler">Handles errors in HTTP responses. Defaults to <see cref="DefaultErrorHandler"/> if unset.</param>
    /// <param name="linkExtractor">Detects links in HTTP responses. Combines <see cref="HeaderLinkExtractor"/> and <see cref="HalLinkExtractor"/> if unset.</param>
    protected EntryEndpoint(HttpClient httpClient, IReadOnlyList<MediaTypeFormatter> serializers, Uri? uri = null, IErrorHandler? errorHandler = null, ILinkExtractor? linkExtractor = null)
        : base(
            (uri ?? httpClient.BaseAddress ?? throw new ArgumentException("uri or httpClient.BaseAddress must be set.", nameof(uri))).EnsureTrailingSlash(),
            httpClient,
            serializers,
            errorHandler ?? new DefaultErrorHandler(),
            linkExtractor ?? new AggregateLinkExtractor(new HeaderLinkExtractor(), new HalLinkExtractor()))
    {
        foreach (var mediaType in Serializers.SelectMany(x=> x.SupportedMediaTypes).Distinct())
        {
            if (mediaType.MediaType != null)
                HttpClient.DefaultRequestHeaders.Accept.Add(new(mediaType.MediaType));
        }
    }

    /// <summary>
    /// Creates a new entry endpoint.
    /// </summary>
    /// <param name="httpClient">The HTTP client used to communicate with the REST API.</param>
    /// <param name="uri">The base URI of the REST API. Missing trailing slash will be appended automatically. <see cref="HttpClient.BaseAddress"/> is used if this is unset.</param>
    /// <param name="serializer">The serializer used for entities sent to and received from the server. Defaults to <see cref="NewtonsoftJsonSerializer"/> if unset.</param>
    /// <param name="errorHandler">Handles errors in HTTP responses. Defaults to <see cref="DefaultErrorHandler"/> if unset.</param>
    /// <param name="linkExtractor">Detects links in HTTP responses. Combines <see cref="HeaderLinkExtractor"/> and <see cref="HalLinkExtractor"/> if unset.</param>
    public EntryEndpoint(HttpClient httpClient, Uri? uri = null, MediaTypeFormatter? serializer = null, IErrorHandler? errorHandler = null, ILinkExtractor? linkExtractor = null)
        : this(httpClient, new[] {serializer ?? new NewtonsoftJsonSerializer()}, uri, errorHandler, linkExtractor)
    {}

    /// <summary>
    /// Creates a new entry endpoint.
    /// </summary>
    /// <param name="uri">The base URI of the REST API.</param>
    /// <param name="credentials">Optional HTTP Basic authentication credentials used to authenticate against the REST API.</param>
    /// <param name="serializer">The serializer used for entities sent to and received from the server. Defaults to <see cref="NewtonsoftJsonSerializer"/> if unset.</param>
    /// <param name="errorHandler">Handles errors in HTTP responses. Leave unset for default implementation.</param>
    /// <param name="linkExtractor">Detects links in HTTP responses. Leave unset for default implementation.</param>
    public EntryEndpoint(Uri uri, NetworkCredential? credentials = null, MediaTypeFormatter? serializer = null, IErrorHandler? errorHandler = null, ILinkExtractor? linkExtractor = null)
        : this(new HttpClient(), uri, serializer, errorHandler, linkExtractor)
    {
        credentials ??= ExtractCredentials(uri);
        if (credentials != null)
            HttpClient.AddBasicAuth(credentials);
    }

    /// <summary>
    /// Extracts credentials from user info in URI if set.
    /// </summary>
    private static NetworkCredential? ExtractCredentials(Uri uri)
    {
        var builder = new UriBuilder(uri);
        return string.IsNullOrEmpty(builder.UserName)
            ? null
            : new NetworkCredential(builder.UserName, builder.Password);
    }

    /// <summary>
    /// Fetches meta data such as links from the server.
    /// </summary>
    /// <exception cref="AuthenticationException"><see cref="HttpStatusCode.Unauthorized"/></exception>
    /// <exception cref="UnauthorizedAccessException"><see cref="HttpStatusCode.Forbidden"/></exception>
    /// <exception cref="KeyNotFoundException"><see cref="HttpStatusCode.NotFound"/> or <see cref="HttpStatusCode.Gone"/></exception>
    /// <exception cref="HttpRequestException">Other non-success status code.</exception>
    public Task ReadMetaAsync(CancellationToken cancellationToken = default)
        => FinalizeAsync(() => HttpClient.GetAsync(Uri, cancellationToken));
}
