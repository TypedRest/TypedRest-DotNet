using System;
using TypedRest.Samples.Library.Models;

namespace TypedRest.Samples.Library.Endpoints
{
    public class SampleEntryEndpoint : EntryEndpoint
    {
        public CollectionEndpoint<Package> Packages { get; private set; }
        public BlobEndpoint Blob { get; private set; }
        public TriggerEndpoint Trigger { get; private set; }

        public SampleEntryEndpoint(Uri uri) : base(uri)
        {
            Packages = new CollectionEndpoint<Package>(this, relativeUri: "packages");
            Blob = new BlobEndpoint(this, relativeUri: "blob");
            Trigger = new TriggerEndpoint(this, relativeUri: "trigger");
        }
    }
}