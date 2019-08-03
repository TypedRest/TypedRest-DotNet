using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace TypedRest.Endpoints.Raw
{
    /// <summary>
    /// Endpoint that accepts binary uploads.
    /// </summary>
    public class UploadEndpoint : EndpointBase, IUploadEndpoint
    {
        private readonly string _formField;

        /// <summary>
        /// Creates a new upload endpoint using raw bodies.
        /// </summary>
        /// <param name="referrer">The endpoint used to navigate to this one.</param>
        /// <param name="relativeUri">The URI of this endpoint relative to the <paramref name="referrer"/>'s.</param>
        public UploadEndpoint(IEndpoint referrer, Uri relativeUri)
            : base(referrer, relativeUri)
        {}

        /// <summary>
        /// Creates a new upload endpoint using multi-part form encoding.
        /// </summary>
        /// <param name="referrer">The endpoint used to navigate to this one.</param>
        /// <param name="relativeUri">The URI of this endpoint relative to the <paramref name="referrer"/>'s.</param>
        /// <param name="formField">The name of the form field to place the uploaded data into.</param>
        public UploadEndpoint(IEndpoint referrer, Uri relativeUri, string formField)
            : this(referrer, relativeUri)
        {
            _formField = formField;
        }

        /// <summary>
        /// Creates a new upload endpoint using raw bodies.
        /// </summary>
        /// <param name="referrer">The endpoint used to navigate to this one.</param>
        /// <param name="relativeUri">The URI of this endpoint relative to the <paramref name="referrer"/>'s. Prefix <c>./</c> to append a trailing slash to the <paramref name="referrer"/> URI if missing.</param>
        public UploadEndpoint(IEndpoint referrer, string relativeUri)
            : base(referrer, relativeUri)
        { }

        /// <summary>
        /// Creates a new upload endpoint using multi-part form encoding.
        /// </summary>
        /// <param name="referrer">The endpoint used to navigate to this one.</param>
        /// <param name="relativeUri">The URI of this endpoint relative to the <paramref name="referrer"/>'s.</param>
        /// <param name="formField">The name of the form field to place the uploaded data into.</param>
        public UploadEndpoint(IEndpoint referrer, string relativeUri, string formField)
            : this(referrer, relativeUri)
        {
            _formField = formField;
        }

        public Task UploadFromAsync(Stream stream, string fileName = null, string mimeType = null, CancellationToken cancellationToken = default)
        {
            HttpContent content = new StreamContent(stream);
            if (!string.IsNullOrEmpty(mimeType)) content.Headers.ContentType = MediaTypeHeaderValue.Parse(mimeType);

            if (!string.IsNullOrEmpty(_formField))
            {
                content = string.IsNullOrEmpty(fileName)
                    ? new MultipartFormDataContent {{content, _formField}}
                    : new MultipartFormDataContent {{content, _formField, fileName}};
            }

            return HandleResponseAsync(HttpClient.PostAsync(Uri, content, cancellationToken));
        }
    }
}
