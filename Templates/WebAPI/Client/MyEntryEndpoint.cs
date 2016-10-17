using System;
using System.Net;
using System.Net.Http.Headers;
using XProjectNamespaceX.Model;
using TypedRest;

namespace XProjectNamespaceX.Client
{
    /// <summary>
    /// Entry point for the XProjectNameX API.
    /// </summary>
    public class MyEntryEndpoint : EntryEndpoint
    {
        /// <summary>
        /// Creates a new XProjectNameX entry point.
        /// </summary>
        /// <param name="uri">The base URI of the REST interface. Missing trailing slash will be appended automatically.</param>
        /// <param name="credentials">HTTP Basic Auth credentials used to authenticate against the REST interface.</param>
        public MyEntryEndpoint(Uri uri, ICredentials credentials)
            : base(uri, credentials)
        {
        }

        /// <summary>
        /// Creates a new XProjectNameX entry point using an OAuth token.
        /// </summary>
        /// <param name="uri">The base URI of the REST interface. Missing trailing slash will be appended automatically.</param>
        /// <param name="token">The OAuth token to present as a "Bearer" to the REST interface.</param>
        public MyEntryEndpoint(Uri uri, string token)
            : base(uri)
        {
            HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        public ICollectionEndpoint<MyEntity> Entities => new CollectionEndpoint<MyEntity>(this, Link("entities"));
    }
}
