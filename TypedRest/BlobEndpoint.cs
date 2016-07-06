using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
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
        /// <param name="referrer">The endpoint used to navigate to this one.</param>
        /// <param name="relativeUri">The URI of this endpoint relative to the <paramref name="referrer"/>'s.</param>
        public BlobEndpoint(IEndpoint referrer, Uri relativeUri)
            : base(referrer, relativeUri)
        {
        }

        /// <summary>
        /// Creates a new blob endpoint.
        /// </summary>
        /// <param name="referrer">The endpoint used to navigate to this one.</param>
        /// <param name="relativeUri">The URI of this endpoint relative to the <paramref name="referrer"/>'s. Prefix <c>./</c> to append a trailing slash to the <paramref name="referrer"/> URI if missing.</param>
        public BlobEndpoint(IEndpoint referrer, string relativeUri)
            : base(referrer, relativeUri)
        {
        }

        public Task ProbeAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            return HandleResponseAsync(HttpClient.OptionsAsync(Uri, cancellationToken));
        }

        public bool? DownloadAllowed => IsVerbAllowed(HttpMethod.Get.Method);

        public async Task<Stream> DownloadAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            var response = await HandleResponseAsync(HttpClient.GetAsync(Uri, cancellationToken)).NoContext();
            return await response.Content.ReadAsStreamAsync().NoContext();
        }

        public bool? UploadAllowed => IsVerbAllowed(HttpMethod.Put.Method);

        public Task UploadFromAsync(Stream stream, string mimeType = null, CancellationToken cancellationToken = new CancellationToken())
        {
            var content = new StreamContent(stream);
            if (!string.IsNullOrEmpty(mimeType)) content.Headers.ContentType = new MediaTypeHeaderValue(mimeType);
            return HandleResponseAsync(HttpClient.PutAsync(Uri, content, cancellationToken));
        }
    }
}