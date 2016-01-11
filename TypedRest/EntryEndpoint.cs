using System;
using System.Net;
using System.Net.Http;

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
            : base(BuildHttpClient(credentials), uri)
        {
        }

        private static HttpClient BuildHttpClient(ICredentials credentials)
        {
            return new HttpClient((credentials == null)
                ? new HttpClientHandler()
                : new HttpClientHandler {PreAuthenticate = true, Credentials = credentials});
        }
    }
}