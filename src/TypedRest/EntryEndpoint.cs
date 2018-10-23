using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TypedRest
{
    /// <summary>
    /// Entry point to a REST interface. Derive from this class and add your own set of child-<see cref="IEndpoint"/>s as properties.
    /// </summary>
    public class EntryEndpoint : EndpointBase
    {
        /// <summary>
        /// Creates a new REST endpoint with an absolute URI.
        /// </summary>
        /// <param name="uri">The base URI of the REST interface. Missing trailing slash will be appended automatically.</param>
        /// <param name="httpClient">The HTTP client used to communicate with the remote element.</param>
        /// <param name="serializer">Controls the serialization of entities sent to and received from the server. Defaults to a JSON serializer if unset.</param>
        public EntryEndpoint(Uri uri, HttpClient httpClient, MediaTypeFormatter serializer = null)
            : base(
                uri.EnsureTrailingSlash(),
                httpClient,
                serializer ?? new DefaultJsonSerializer())
        {}

        /// <summary>
        /// Creates a new entry point.
        /// </summary>
        /// <param name="uri">The base URI of the REST interface.</param>
        /// <param name="credentials">Optional HTTP Basic Auth credentials used to authenticate against the REST interface.</param>
        /// <param name="serializer">Controls the serialization of entities sent to and received from the server. Defaults to a JSON serializer if unset.</param>
        public EntryEndpoint(Uri uri, ICredentials credentials = null, MediaTypeFormatter serializer = null)
            : this(uri, new HttpClient(), serializer)
        {
            BasicAuth(uri, credentials);
        }

        /// <summary>
        /// Creates a new entry point using an OAuth token.
        /// </summary>
        /// <param name="uri">The base URI of the REST interface.</param>
        /// <param name="token">The OAuth token to present as a "Bearer" to the REST interface.</param>
        /// <param name="serializer">Controls the serialization of entities sent to and received from the server. Defaults to a JSON serializer if unset.</param>
        public EntryEndpoint(Uri uri, string token, MediaTypeFormatter serializer = null)
            : this(uri, new HttpClient(), serializer)
        {
            BearerAuth(token);
        }

        private void BasicAuth(Uri uri, ICredentials credentials)
        {
            string userInfo = (credentials == null) ? uri.UserInfo : GetUserInfo(credentials);
            if (userInfo != null)
            {
                HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",
                    Convert.ToBase64String(Encoding.GetEncoding("iso-8859-1").GetBytes(userInfo)));
            }
        }

        private string GetUserInfo(ICredentials credentials)
        {
            var basicCredentials = credentials.GetCredential(Uri, authType: "Basic");
            return (basicCredentials == null)
                ? null
                : basicCredentials.UserName + ":" + basicCredentials.Password;
        }

        private void BearerAuth(string token)
            => HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        /// <summary>
        /// Sets a custom HTTP header to be sent with all requests. Inherited by child endpoints.
        /// </summary>
        /// <param name="name">The name of the header.</param>
        /// <param name="value">The value for the header.</param>
        /// <remarks>Only use this for non-default headers. For default headers use the properties on <see cref="EndpointBase.HttpClient"/>.<see cref="HttpClient.DefaultRequestHeaders"/>.</remarks>
        protected void SetCustomHeader(string name, string value)
        {
            HttpClient.DefaultRequestHeaders.Remove(name);
            HttpClient.DefaultRequestHeaders.Add(name, value);
        }

        /// <summary>
        /// Fetches meta data such as links from the server.
        /// </summary>
        /// <exception cref="AuthenticationException"><see cref="HttpStatusCode.Unauthorized"/></exception>
        /// <exception cref="UnauthorizedAccessException"><see cref="HttpStatusCode.Forbidden"/></exception>
        /// <exception cref="KeyNotFoundException"><see cref="HttpStatusCode.NotFound"/> or <see cref="HttpStatusCode.Gone"/></exception>
        /// <exception cref="HttpRequestException">Other non-success status code.</exception>
        public Task ReadMetaAsync(CancellationToken cancellationToken = default)
            => HandleResponseAsync(HttpClient.GetAsync(Uri, cancellationToken));
    }
}
