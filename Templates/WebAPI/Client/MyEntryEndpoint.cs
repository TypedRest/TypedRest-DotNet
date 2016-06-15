using System;
using System.Configuration;
using System.Net;
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
        /// Configures a connection to the XProjectNameX API.
        /// </summary>
        /// <param name="uri">The base URI of the REST interface. Missing trailing slash will be appended automatically.</param>
        /// <param name="credentials">The credentials used to authenticate against the REST interface.</param>
        public MyEntryEndpoint(Uri uri, ICredentials credentials)
            : base(uri, credentials)
        {
        }

        /// <summary>
        /// Configures a connection to the XProjectNameX API using credentials from ConnectionStrings.
        /// </summary>
        public MyEntryEndpoint()
            : base(new Uri(ConfigurationManager.ConnectionStrings["XProjectNamespaceX"].ConnectionString))
        {
        }

        public ICollectionEndpoint<MyEntity> Entities => new CollectionEndpoint<MyEntity>(this, "entities");
    }
}