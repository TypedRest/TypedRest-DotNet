using System;
using TypedRest.Samples.Library.Models;

namespace TypedRest.Samples.Library.Endpoints
{
    /// <summary>
    /// REST endpoint that represents a <see cref="ResourceRevision"/>.
    /// </summary>
    public class ResourceRevisionElement : ElementEndpoint<ResourceRevision>
    {
        /// <summary>
        /// Represents the blob/file backing the <see cref="ResourceRevision"/>.
        /// </summary>
        public IBlobEndpoint Blob => new BlobEndpoint(this, relativeUri: "blob");

        /// <summary>
        /// Promotes the <see cref="ResourceRevision"/> to the next stage.
        /// </summary>
        public ITriggerEndpoint Promote => new TriggerEndpoint(this, relativeUri: "promote");

        public ResourceRevisionElement(ResourceRevisionCollection parent, Uri relativeUri) : base(parent, relativeUri)
        {
        }

        public ResourceRevisionElement(ResourceRevisionCollection parent, string relativeUri)
            : base(parent, relativeUri)
        {
        }
    }
}