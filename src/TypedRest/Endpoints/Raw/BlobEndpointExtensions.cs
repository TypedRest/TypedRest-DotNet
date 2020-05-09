using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Security.Authentication;
using System.Threading;
using System.Threading.Tasks;

namespace TypedRest.Endpoints.Raw
{
    /// <summary>
    /// Provides extension methods for <see cref="IBlobEndpoint"/>.
    /// </summary>
    public static class BlobEndpointExtensions
    {
        /// <summary>
        /// Downloads the blob's content to a file.
        /// </summary>
        /// <param name="endpoint">The blob endpoint.</param>
        /// <param name="path">The path of the file to write the download data to.</param>
        /// <param name="cancellationToken">Used to cancel the request.</param>
        /// <returns>A stream with the blob's content.</returns>
        /// <exception cref="InvalidDataException"><see cref="HttpStatusCode.BadRequest"/></exception>
        /// <exception cref="AuthenticationException"><see cref="HttpStatusCode.Unauthorized"/></exception>
        /// <exception cref="UnauthorizedAccessException"><see cref="HttpStatusCode.Forbidden"/></exception>
        /// <exception cref="KeyNotFoundException"><see cref="HttpStatusCode.NotFound"/> or <see cref="HttpStatusCode.Gone"/></exception>
        /// <exception cref="InvalidOperationException"><see cref="HttpStatusCode.Conflict"/></exception>
        /// <exception cref="HttpRequestException">Other non-success status code.</exception>
        public static async Task DownloadAsync(this IBlobEndpoint endpoint, string path, CancellationToken cancellationToken = default)
        {
            using (var fileStream = File.Create(path))
            using (var downloadStream = await endpoint.DownloadAsync(cancellationToken))
                await downloadStream.CopyToAsync(fileStream);
        }

        /// <summary>
        /// Uploads content as the blob's content from a file.
        /// </summary>
        /// <param name="endpoint">The blob endpoint.</param>
        /// <param name="path">The path of the file to read the upload data from.</param>
        /// <param name="mimeType">The MIME type of the data to upload.</param>
        /// <param name="cancellationToken">Used to cancel the request.</param>
        /// <exception cref="InvalidDataException"><see cref="HttpStatusCode.BadRequest"/></exception>
        /// <exception cref="AuthenticationException"><see cref="HttpStatusCode.Unauthorized"/></exception>
        /// <exception cref="UnauthorizedAccessException"><see cref="HttpStatusCode.Forbidden"/></exception>
        /// <exception cref="KeyNotFoundException"><see cref="HttpStatusCode.NotFound"/> or <see cref="HttpStatusCode.Gone"/></exception>
        /// <exception cref="InvalidOperationException"><see cref="HttpStatusCode.Conflict"/></exception>
        /// <exception cref="HttpRequestException">Other non-success status code.</exception>
        public static async Task UploadFromAsync(this IBlobEndpoint endpoint, string path, string? mimeType = null, CancellationToken cancellationToken = default)
        {
            using (var fileStream = File.OpenRead(path))
                await endpoint.UploadFromAsync(fileStream, mimeType, cancellationToken);
        }
    }
}
