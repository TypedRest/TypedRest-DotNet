using System;
using System.Configuration;
using System.Net;
using System.Net.Http.Headers;
using XProjectNamespaceX.Model;
using TypedRest;

namespace XProjectNamespaceX.Client
{
    /// <summary>
    /// Entry point for XProjectNameX API.
    /// </summary>
    public class MyEntryEndpoint : EntryEndpoint
    {
        /// <summary>
        /// Creates a new XProjectNameX entry point.
        /// </summary>
        /// <param name="uri">The base URI of the REST interface. Missing trailing slash will be appended automatically.</param>
        /// <param name="credentials">The credentials used to authenticate against the REST interface. Extracts credentials from <paramref name="uri"/> is unset.</param>
        public MyEntryEndpoint(Uri uri, ICredentials credentials = null)
            : base(uri, credentials)
        {
        }

        /// <summary>
        /// Creates a new XProjectNameX entry point using URI (with credentials) from ConnectionStrings.
        /// </summary>
        public MyEntryEndpoint()
            : this(new Uri(ConfigurationManager.ConnectionStrings["XProjectNamespaceX"].ConnectionString))
        {
        }

        /// <summary>
        /// Creates a new XProjectNameX entry point using a token for authentication.
        /// </summary>
        /// <param name="uri">The base URI of the REST interface. Missing trailing slash will be appended automatically.</param>
        /// <param name="token">The token used to authenticate against the REST interface.</param>
        public MyEntryEndpoint(Uri uri, string token)
            : base(uri)
        {
            HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        /// <summary>
        /// Creates a new XProjectNameX entry point using URI from ConnectionStrings and a token for authentication.
        /// </summary>
        public MyEntryEndpoint(string token)
            : this(new Uri(ConfigurationManager.ConnectionStrings["XProjectNamespaceX"].ConnectionString), token)
        {
        }

        public ICollectionEndpoint<MyEntity> Entities => new CollectionEndpoint<MyEntity>(this, Link("entities"));
    }
}
