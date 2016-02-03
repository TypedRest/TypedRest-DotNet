using System;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace TypedRest
{
    /// <summary>
    /// REST endpoint that represents a single binary blob that can downloaded and uploaded.
    /// </summary>
    public class BlobEndpoint : EndpointBase, IBlobEndpoint
    {
        /// <summary>
        /// Creates a new blob endpoint.
        /// </summary>
        /// <param name="parent">The parent endpoint containing this one.</param>
        /// <param name="relativeUri">The URI of this endpoint relative to the <paramref name="parent"/>'s.</param>
        public BlobEndpoint(IEndpoint parent, Uri relativeUri)
            : base(parent, relativeUri)
        {
        }

        /// <summary>
        /// Creates a new blob endpoint.
        /// </summary>
        /// <param name="parent">The parent endpoint containing this one.</param>
        /// <param name="relativeUri">The URI of this endpoint relative to the <paramref name="parent"/>'s.</param>
        public BlobEndpoint(IEndpoint parent, string relativeUri)
            : base(parent, relativeUri)
        {
        }

        public Task ProbeAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            return HandleResponseAsync(HttpClient.OptionsAsync(Uri, cancellationToken));
        }

        public bool? DownloadAllowed => IsVerbAllowed(HttpMethod.Get.Method);

        public async Task<string> DownloadToAsync(Stream stream)
        {
            var response = await HandleResponseAsync(HttpClient.GetAsync(Uri)).NoContext();

            await response.Content.CopyToAsync(stream).NoContext();
            return response.Content.Headers.ContentType.MediaType;
        }

        public bool? UploadAllowed => IsVerbAllowed(HttpMethod.Put.Method);

        public Task UploadFromAsync(Stream stream)
        {
            return HandleResponseAsync(HttpClient.PutAsync(Uri, new StreamContent(stream)));
        }
    }
}