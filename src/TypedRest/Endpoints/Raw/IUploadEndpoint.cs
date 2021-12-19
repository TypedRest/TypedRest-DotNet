namespace TypedRest.Endpoints.Raw;

/// <summary>
/// Endpoint that accepts binary uploads.
/// </summary>
public interface IUploadEndpoint : IEndpoint
{
    /// <summary>
    /// Uploads data to the endpoint from a stream.
    /// </summary>
    /// <param name="stream">The stream to read the upload data from.</param>
    /// <param name="fileName">The name of the uploaded file.</param>
    /// <param name="mimeType">The MIME type of the data to upload.</param>
    /// <param name="cancellationToken">Used to cancel the request.</param>
    /// <exception cref="InvalidDataException"><see cref="HttpStatusCode.BadRequest"/></exception>
    /// <exception cref="AuthenticationException"><see cref="HttpStatusCode.Unauthorized"/></exception>
    /// <exception cref="UnauthorizedAccessException"><see cref="HttpStatusCode.Forbidden"/></exception>
    /// <exception cref="KeyNotFoundException"><see cref="HttpStatusCode.NotFound"/> or <see cref="HttpStatusCode.Gone"/></exception>
    /// <exception cref="HttpRequestException">Other non-success status code.</exception>
    Task UploadFromAsync(Stream stream, string? fileName = null, string? mimeType = null, CancellationToken cancellationToken = default);
}