using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
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
        /// Creates a new REST interface.
        /// </summary>
        /// <param name="uri">The base URI of the REST interface. Missing trailing slash will be appended automatically.</param>
        /// <param name="credentials">The credentials used to authenticate against the REST interface. Can be <c>null</c> for no authentication.</param>
        public EntryEndpoint(Uri uri, ICredentials credentials = null)
            : base(BuildHttpClient(uri, credentials), uri.EnsureTrailingSlash())
        {
        }

        private static HttpClient BuildHttpClient(Uri uri, ICredentials credentials)
        {
            if (credentials == null) return new HttpClient();

            var handler = new HttpClientHandler {PreAuthenticate = true, Credentials = credentials};

            var basicCredentials = credentials.GetCredential(uri, "Basic");
            if (basicCredentials == null) return new HttpClient(handler);

            return new HttpClient(handler)
            {
                DefaultRequestHeaders =
                {
                    // Preemptively set HTTP Basic Auth header when appropriate
                    Authorization = new AuthenticationHeaderValue(
                        "Basic",
                        Convert.ToBase64String(
                            Encoding.ASCII.GetBytes(basicCredentials.UserName + ":" + basicCredentials.Password)))
                }
            };
        }

        /// <summary>
        /// Fetches meta data such as links from the server.
        /// </summary>
        /// <exception cref="UnauthorizedAccessException"><see cref="HttpStatusCode.Unauthorized"/> or <see cref="HttpStatusCode.Forbidden"/></exception>
        /// <exception cref="KeyNotFoundException"><see cref="HttpStatusCode.NotFound"/> or <see cref="HttpStatusCode.Gone"/></exception>
        /// <exception cref="HttpRequestException">Other non-success status code.</exception>
        public Task ReadMetaAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            return HandleResponseAsync(HttpClient.GetAsync(Uri, cancellationToken));
        }
    }
}