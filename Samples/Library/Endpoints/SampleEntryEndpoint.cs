using System;
using System.Net;
using TypedRest.Samples.Library.Models;

namespace TypedRest.Samples.Library.Endpoints
{
    /// <summary>
    /// Entry point for the IRUS REST API.
    /// </summary>
    public class SampleEntryEndpoint : EntryEndpoint
    {
        public TriggerEndpoint TestData => new TriggerEndpoint(this, relativeUri: "test-data");
        public ResourceCollection Resources => new ResourceCollection(this);
        public PagedResourceCollection ResourcesPaged => new PagedResourceCollection(this);
        public CollectionEndpoint<Target> Targets => new CollectionEndpoint<Target>(this, relativeUri: "targets");

        public SampleEntryEndpoint(Uri uri, ICredentials credentials = null) : base(uri, credentials)
        {
        }
    }
}