namespace TypedRest.Endpoints.Raw;

/// <summary>
/// Endpoint for a binary blob that can be downloaded or uploaded.
/// </summary>
public interface IBlobEndpoint : IEndpoint
{
    /// <summary>
    /// Queries the server about capabilities of the endpoint without performing any action.
    /// </summary>
    /// <param name="cancellationToken">Used to cancel the request.</param>
    /// <exception cref="AuthenticationException"><see cref="HttpStatusCode.Unauthorized"/></exception>
    /// <exception cref="UnauthorizedAccessException"><see cref="HttpStatusCode.Forbidden"/></exception>
    /// <exception cref="KeyNotFoundException"><see cref="HttpStatusCode.NotFound"/> or <see cref="HttpStatusCode.Gone"/></exception>
    /// <exception cref="HttpRequestException">Other non-success status code.</exception>
    Task ProbeAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Indicates whether the server has specified <see cref="DownloadAsync"/> is currently allowed.
    /// </summary>
    /// <remarks>Uses cached data from last response.</remarks>
    /// <returns><c>true</c> if the method is allowed, <c>false</c> if the method is not allowed, <c>null</c> If no request has been sent yet or the server did not specify allowed methods.</returns>
    bool? DownloadAllowed { get; }

    /// <summary>
    /// Downloads the blob's content to a stream.
    /// </summary>
    /// <param name="cancellationToken">Used to cancel the request.</param>
    /// <returns>A stream with the blob's content.</returns>
    /// <exception cref="AuthenticationException"><see cref="HttpStatusCode.Unauthorized"/></exception>
    /// <exception cref="UnauthorizedAccessException"><see cref="HttpStatusCode.Forbidden"/></exception>
    /// <exception cref="KeyNotFoundException"><see cref="HttpStatusCode.NotFound"/> or <see cref="HttpStatusCode.Gone"/></exception>
    /// <exception cref="HttpRequestException">Other non-success status code.</exception>
    Task<Stream> DownloadAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Indicates whether the server has specified <see cref="UploadFromAsync"/> is currently allowed.
    /// </summary>
    /// <remarks>Uses cached data from last response.</remarks>
    /// <returns><c>true</c> if the method is allowed, <c>false</c> if the method is not allowed, <c>null</c> If no request has been sent yet or the server did not specify allowed methods.</returns>
    bool? UploadAllowed { get; }

    /// <summary>
    /// Uploads data as the blob's content from a stream.
    /// </summary>
    /// <param name="stream">The stream to read the upload data from.</param>
    /// <param name="mimeType">The MIME type of the data to upload.</param>
    /// <param name="cancellationToken">Used to cancel the request.</param>
    /// <exception cref="InvalidDataException"><see cref="HttpStatusCode.BadRequest"/></exception>
    /// <exception cref="AuthenticationException"><see cref="HttpStatusCode.Unauthorized"/></exception>
    /// <exception cref="UnauthorizedAccessException"><see cref="HttpStatusCode.Forbidden"/></exception>
    /// <exception cref="HttpRequestException">Other non-success status code.</exception>
    Task UploadFromAsync(Stream stream, string? mimeType = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Indicates whether the server has specified <see cref="DeleteAsync"/> is currently allowed.
    /// </summary>
    /// <remarks>Uses cached data from last response.</remarks>
    /// <returns><c>true</c> if the method is allowed, <c>false</c> if the method is not allowed, <c>null</c> If no request has been sent yet or the server did not specify allowed methods.</returns>
    bool? DeleteAllowed { get; }

    /// <summary>
    /// Deletes the blob from the server.
    /// </summary>
    /// <param name="cancellationToken">Used to cancel the request.</param>
    /// <exception cref="InvalidDataException"><see cref="HttpStatusCode.BadRequest"/></exception>
    /// <exception cref="AuthenticationException"><see cref="HttpStatusCode.Unauthorized"/></exception>
    /// <exception cref="UnauthorizedAccessException"><see cref="HttpStatusCode.Forbidden"/></exception>
    /// <exception cref="KeyNotFoundException"><see cref="HttpStatusCode.NotFound"/> or <see cref="HttpStatusCode.Gone"/></exception>
    /// <exception cref="HttpRequestException">Other non-success status code.</exception>
    Task DeleteAsync(CancellationToken cancellationToken = default);
}
