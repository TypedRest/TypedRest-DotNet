using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Security.Authentication;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace TypedRest
{
    /// <summary>
    /// Entry point to a REST interface. Derive from this class and add your own set of child-<see cref="IEndpoint"/>s as properties.
    /// </summary>
    public class EntryEndpoint : EndpointBase
    {
        /// <summary>
        /// Creates a new entry point.
        /// </summary>
        /// <param name="uri">The base URI of the REST interface. Missing trailing slash will be appended automatically.</param>
        /// <param name="credentials">Optional HTTP Basic Auth credentials used to authenticate against the REST interface.</param>
        /// <param name="serializer">Controls the serialization of entities sent to and received from the server. Defaults to a JSON serializer if unset.</param>
        public EntryEndpoint(Uri uri, ICredentials credentials = null, MediaTypeFormatter serializer = null) : base(
            uri: uri.EnsureTrailingSlash(),
            httpClient: BuildHttpClient(uri, credentials),
            serializer: serializer ?? BuildSerializer())
        {
        }

        /// <summary>
        /// Creates a new entry point using an OAuth token.
        /// </summary>
        /// <param name="uri">The base URI of the REST interface. Missing trailing slash will be appended automatically.</param>
        /// <param name="token">The OAuth token to present as a "Bearer" to the REST interface.</param>
        /// <param name="serializer">Controls the serialization of entities sent to and received from the server. Defaults to a JSON serializer if unset.</param>
        public EntryEndpoint(Uri uri, string token, MediaTypeFormatter serializer = null) : base(
            uri: uri.EnsureTrailingSlash(),
            httpClient: BuildHttpClient(uri),
            serializer: serializer ?? BuildSerializer())
        {
            HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        private static HttpClient BuildHttpClient(Uri uri, ICredentials credentials = null)
        {
            var handler = new HttpClientHandler();
            var client = new HttpClient(handler);

            if (credentials != null)
            {
                handler.PreAuthenticate = true;
                handler.Credentials = credentials;

                var basicCredentials = credentials.GetCredential(uri, authType: "Basic");
                if (basicCredentials != null)
                    SetBasicAuthHeader(client, basicCredentials);
            }

            return client;
        }

        private static void SetBasicAuthHeader(HttpClient client, NetworkCredential basicCredentials)
        {
            string encodedCredentials = Convert.ToBase64String(Encoding.GetEncoding("iso-8859-1")
                .GetBytes(basicCredentials.UserName + ":" + basicCredentials.Password));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", encodedCredentials);
        }

        private static MediaTypeFormatter BuildSerializer()
        {
            return new JsonMediaTypeFormatter
            {
                SerializerSettings =
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver(),
                    Converters = {new StringEnumConverter {CamelCaseText = true}},
                    NullValueHandling = NullValueHandling.Ignore,
                    TypeNameHandling = TypeNameHandling.Auto
                }
            };
        }

        /// <summary>
        /// Fetches meta data such as links from the server.
        /// </summary>
        /// <exception cref="InvalidCredentialException"><see cref="HttpStatusCode.Unauthorized"/></exception>
        /// <exception cref="UnauthorizedAccessException"><see cref="HttpStatusCode.Forbidden"/></exception>
        /// <exception cref="KeyNotFoundException"><see cref="HttpStatusCode.NotFound"/> or <see cref="HttpStatusCode.Gone"/></exception>
        /// <exception cref="HttpRequestException">Other non-success status code.</exception>
        public Task ReadMetaAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            return HandleResponseAsync(HttpClient.GetAsync(Uri, cancellationToken));
        }
    }
}