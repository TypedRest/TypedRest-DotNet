using System;
using System.IO;
using System.Net.Http;
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

        public bool? DownloadAllowed => IsVerbAllowed("GET");

        public async Task DownloadToAsync(Stream stream)
        {
            var response = await HandleResponseAsync(HttpClient.GetAsync(Uri));

            await response.Content.CopyToAsync(stream);
        }

        public bool? UploadAllowed => IsVerbAllowed("PUT");

        public async Task UploadFromAsync(Stream stream)
        {
            await HandleResponseAsync(HttpClient.PutAsync(Uri, new StreamContent(stream)));
        }
    }
}