namespace TypedRest.Endpoints.Raw;

/// <summary>
/// Endpoint that accepts binary uploads using multi-part form encoding or raw bodies.
/// </summary>
/// <param name="referrer">The endpoint used to navigate to this one.</param>
/// <param name="relativeUri">The URI of this endpoint relative to the <paramref name="referrer"/>'s.</param>
/// <param name="formField">The name of the form field to place the uploaded data into; leave <c>null</c> to use raw bodies instead of a multi-part forms.</param>
public class UploadEndpoint(IEndpoint referrer, Uri relativeUri, string? formField = null)
    : EndpointBase(referrer, relativeUri), IUploadEndpoint
{
    /// <summary>
    /// Creates a new upload endpoint using multi-part form encoding.
    /// </summary>
    /// <param name="referrer">The endpoint used to navigate to this one.</param>
    /// <param name="relativeUri">The URI of this endpoint relative to the <paramref name="referrer"/>'s.</param>
    /// <param name="formField">The name of the form field to place the uploaded data into; leave <c>null</c> to use raw bodies instead of a multi-part forms.</param>
    public UploadEndpoint(IEndpoint referrer, string relativeUri, string? formField = null)
        : this(referrer, new Uri(relativeUri, UriKind.Relative), formField) {}

    public async Task UploadFromAsync(Stream stream, string? fileName = null, string? mimeType = null, CancellationToken cancellationToken = default)
    {
        HttpContent content = new StreamContent(stream);
        if (!string.IsNullOrEmpty(mimeType)) content.Headers.ContentType = MediaTypeHeaderValue.Parse(mimeType);

        if (!string.IsNullOrEmpty(formField))
        {
            content = string.IsNullOrEmpty(fileName)
                ? new MultipartFormDataContent {{content, formField}}
                : new MultipartFormDataContent {{content, formField, fileName}};
        }

        await FinalizeAsync(() => HttpClient.PostAsync(Uri, content, cancellationToken)).NoContext();
    }
}
