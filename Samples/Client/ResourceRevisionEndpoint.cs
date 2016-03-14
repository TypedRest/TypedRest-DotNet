using System;
using TypedRest.Samples.Model;

namespace TypedRest.Samples.Client
{
    /// <summary>
    /// REST endpoint that represents a <see cref="ResourceRevision"/>.
    /// </summary>
    public class ResourceRevisionEndpoint : ElementEndpoint<ResourceRevision>
    {
        public ResourceRevisionEndpoint(ResourceRevisionCollectionEndpoint parent, Uri relativeUri)
            : base(parent, relativeUri)
        {
        }

        /// <summary>
        /// Represents the blob/file backing the <see cref="ResourceRevision"/>.
        /// </summary>
        public IBlobEndpoint Blob => new BlobEndpoint(this, Link("blob"));

        /// <summary>
        /// Promotes the <see cref="ResourceRevision"/> to the next stage.
        /// </summary>
        public ITriggerEndpoint Promote => new TriggerEndpoint(this, Link("promote"));
    }
}