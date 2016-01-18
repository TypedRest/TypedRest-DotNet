using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace TypedRest
{
    /// <summary>
    /// REST endpoint that represents a single binary blob that can downloaded and uploaded.
    /// </summary>
    public interface IBlobEndpoint : IEndpoint
    {
        /// <summary>
        /// Shows whether the server has indicated that <seealso cref="DownloadToAsync"/> is currently allowed.
        /// If the server did not specify anything <c>null</c> is returned.
        /// </summary>
        /// <remarks>Uses cached data from last response if possible. Tries lazy lookup with HTTP OPTIONS if no requests have been performed yet.</remarks>
        bool? DownloadAllowed { get; }

        /// <summary>
        /// Downloads the blob's content.
        /// </summary>
        /// <param name="stream">The stream to write the downloaded data to.</param>
        /// <exception cref="InvalidDataException"><see cref="HttpStatusCode.BadRequest"/></exception>
        /// <exception cref="UnauthorizedAccessException"><see cref="HttpStatusCode.Unauthorized"/> or <see cref="HttpStatusCode.Forbidden"/></exception>
        /// <exception cref="KeyNotFoundException"><see cref="HttpStatusCode.NotFound"/> or <see cref="HttpStatusCode.Gone"/></exception>
        /// <exception cref="InvalidOperationException"><see cref="HttpStatusCode.Conflict"/></exception>
        /// <exception cref="HttpRequestException">Other non-success status code.</exception>
        Task DownloadToAsync(Stream stream);

        /// <summary>
        /// Shows whether the server has indicated that <seealso cref="UploadFromAsync"/> is currently allowed.
        /// If the server did not specify anything <c>null</c> is returned.
        /// </summary>
        /// <remarks>Uses cached data from last response if possible. Tries lazy lookup with HTTP OPTIONS if no requests have been performed yet.</remarks>
        bool? UploadAllowed { get; }

        /// <summary>
        /// Uploads content as the blob's content.
        /// </summary>
        /// <param name="stream">The stream to read the upload data from.</param>
        /// <exception cref="InvalidDataException"><see cref="HttpStatusCode.BadRequest"/></exception>
        /// <exception cref="UnauthorizedAccessException"><see cref="HttpStatusCode.Unauthorized"/> or <see cref="HttpStatusCode.Forbidden"/></exception>
        /// <exception cref="KeyNotFoundException"><see cref="HttpStatusCode.NotFound"/> or <see cref="HttpStatusCode.Gone"/></exception>
        /// <exception cref="InvalidOperationException"><see cref="HttpStatusCode.Conflict"/></exception>
        /// <exception cref="HttpRequestException">Other non-success status code.</exception>
        Task UploadFromAsync(Stream stream);
    }
}