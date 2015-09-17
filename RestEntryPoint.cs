using System;
using System.Net;
using System.Net.Http;

namespace TypedRest
{
    /// <summary>
    /// Entry point to a REST interface.
    /// </summary>
    public abstract class RestEntryPoint : RestEndpoint
    {
        /// <summary>
        /// Creates a new REST interface.
        /// </summary>
        /// <param name="uri">The base URI of the REST interface. Missing trailing slash will be appended automatically.</param>
        /// <param name="credentials">The credentials used to authenticate against the REST interface. Can be <see langword="null"/> for no authentication.</param>
        protected RestEntryPoint(Uri uri, ICredentials credentials = null)
            : base(new HttpClient((credentials == null)
                ? new HttpClientHandler()
                : new HttpClientHandler {PreAuthenticate = true, Credentials = credentials})
            {BaseAddress = uri.EnsureTrailingSlash()}, uri.EnsureTrailingSlash())
        {
        }
    }
}