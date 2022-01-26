namespace TypedRest.Endpoints.Raw;

/// <summary>
/// Endpoint for a binary blob that can be downloaded or uploaded.
/// </summary>
public class BlobEndpoint : EndpointBase, IBlobEndpoint
{
    /// <summary>
    /// Creates a new blob endpoint.
    /// </summary>
    /// <param name="referrer">The endpoint used to navigate to this one.</param>
    /// <param name="relativeUri">The URI of this endpoint relative to the <paramref name="referrer"/>'s.</param>
    public BlobEndpoint(IEndpoint referrer, Uri relativeUri)
        : base(referrer, relativeUri)
    {}

    /// <summary>
    /// Creates a new blob endpoint.
    /// </summary>
    /// <param name="referrer">The endpoint used to navigate to this one.</param>
    /// <param name="relativeUri">The URI of this endpoint relative to the <paramref name="referrer"/>'s. Add a <c>./</c> prefix here to imply a trailing slash <paramref name="referrer"/>'s URI.</param>
    public BlobEndpoint(IEndpoint referrer, string relativeUri)
        : base(referrer, relativeUri)
    {}

    public Task ProbeAsync(CancellationToken cancellationToken = default)
        => FinalizeAsync(async () => await HttpClient.OptionsAsync(Uri, cancellationToken));

    public bool? DownloadAllowed => IsMethodAllowed(HttpMethod.Get);

    public async Task<Stream> DownloadAsync(CancellationToken cancellationToken = default)
    {
        var response = await HandleAsync(() => HttpClient.GetAsync(Uri, HttpCompletionOption.ResponseHeadersRead, cancellationToken)).NoContext();
        return await response.Content.ReadAsStreamAsync(
#if NET
            cancellationToken
#endif
        ).NoContext();
    }

    public bool? UploadAllowed => IsMethodAllowed(HttpMethod.Put);

    public async Task UploadFromAsync(Stream stream, string? mimeType = null, CancellationToken cancellationToken = default)
    {
        var content = new StreamContent(stream);
        if (!string.IsNullOrEmpty(mimeType)) content.Headers.ContentType = MediaTypeHeaderValue.Parse(mimeType);
        await FinalizeAsync(() => HttpClient.PutAsync(Uri, content, cancellationToken)).NoContext();
    }

    public bool? DeleteAllowed => IsMethodAllowed(HttpMethod.Delete);

    public Task DeleteAsync(CancellationToken cancellationToken = default)
        => FinalizeAsync(() => HttpClient.DeleteAsync(Uri, cancellationToken));
}