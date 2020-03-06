using System;
using System.Net.Http;
using System.Net.Http.Formatting;
using TypedRest.Errors;
using TypedRest.Http;
using TypedRest.Links;

namespace TypedRest.Endpoints
{
    /// <summary>
    /// Entry point to a REST API protected with OAuth 2.0. Derive from this class and add your own set of child-<see cref="IEndpoint"/>s as properties.
    /// </summary>
    /// <seealso cref="OAuthHandler"/>
    public class OAuthEntryEndpoint : EntryEndpoint
    {
        /// <summary>
        /// Creates a new endpoint with an absolute URI.
        /// </summary>
        /// <param name="uri">The base URI of the REST API. Missing trailing slash will be appended automatically.</param>
        /// <param name="oAuthOptions">Options for OAuth 2.0 / OpenID Connect authentication. (optional)</param>
        /// <param name="httpMessageHandler">The HTTP message handler used to communicate with the remote element. (optional)</param>
        /// <param name="serializer">Controls the serialization of entities sent to and received from the server. Defaults to a JSON serializer if unset.</param>
        /// <param name="errorHandler">Handles errors in HTTP responses. Leave unset for default implementation.</param>
        /// <param name="linkHandler">Detects links in HTTP responses. Leave unset for default implementation.</param>
        public OAuthEntryEndpoint(Uri uri, OAuthOptions? oAuthOptions = null, HttpMessageHandler? httpMessageHandler = null, MediaTypeFormatter? serializer = null, IErrorHandler? errorHandler = null, ILinkHandler? linkHandler = null)
            : base(
                uri,
                new HttpClient(oAuthOptions == null
                    ? httpMessageHandler ?? new HttpClientHandler()
                    : new OAuthHandler(oAuthOptions, httpMessageHandler ?? new HttpClientHandler())),
                serializer,
                errorHandler,
                linkHandler)
        {}
    }
}
