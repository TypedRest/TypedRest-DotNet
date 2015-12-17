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
        public SampleEntryEndpoint(Uri uri, ICredentials credentials = null) : base(uri, credentials)
        {
        }

        public ResourceCollection Resources => new ResourceCollection(this);

        public CollectionEndpoint<Target> Targets => new CollectionEndpoint<Target>(this, Link("targets"));
    }
}