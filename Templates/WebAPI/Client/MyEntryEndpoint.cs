using System;
using System.Net;
using XProjectNamespaceX.Model;
using TypedRest;

namespace XProjectNamespaceX.Client
{
    public class MyEntryEndpoint : EntryEndpoint
    {
        public MyEntryEndpoint(Uri uri, ICredentials credentials = null) : base(uri, credentials)
        {
        }

        public ICollectionEndpoint<MyEntity> Entities
        {
            get { return new CollectionEndpoint<MyEntity>(this, "entities"); }
        }
    }
}
