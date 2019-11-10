using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using TypedRest.Http;

namespace TypedRest.Endpoints.Raw
{
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
        /// <param name="relativeUri">The URI of this endpoint relative to the <paramref name="referrer"/>'s. Prefix <c>./</c> to append a trailing slash to the <paramref name="referrer"/> URI if missing.</param>
        public BlobEndpoint(IEndpoint referrer, string relativeUri)
            : base(referrer, relativeUri)
        {}

        public Task ProbeAsync(CancellationToken cancellationToken = default)
            => HandleResponseAsync(HttpClient.OptionsAsync(Uri, cancellationToken));

        public bool? DownloadAllowed => IsMethodAllowed(HttpMethod.Get);

        public async Task<Stream> DownloadAsync(CancellationToken cancellationToken = default)
        {
            var response = await HandleResponseAsync(HttpClient.GetAsync(Uri, HttpCompletionOption.ResponseHeadersRead, cancellationToken)).NoContext();
            return await response.Content.ReadAsStreamAsync().NoContext();
        }

        public bool? UploadAllowed => IsMethodAllowed(HttpMethod.Put);

        public Task UploadFromAsync(Stream stream, string? mimeType = null, CancellationToken cancellationToken = default)
        {
            var content = new StreamContent(stream);
            if (!string.IsNullOrEmpty(mimeType)) content.Headers.ContentType = MediaTypeHeaderValue.Parse(mimeType);
            return HandleResponseAsync(HttpClient.PutAsync(Uri, content, cancellationToken));
        }

        public bool? DeleteAllowed => IsMethodAllowed(HttpMethod.Delete);

        public Task DeleteAsync(CancellationToken cancellationToken = default)
            => HandleResponseAsync(HttpClient.DeleteAsync(Uri, cancellationToken));
    }
}
