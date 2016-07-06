using System;
using TypedRest;
using TypedRestSample.Model;

namespace TypedRestSample.Client
{
    /// <summary>
    /// REST endpoint that represents a <see cref="ResourceRevision"/>.
    /// </summary>
    public class ResourceRevisionEndpoint : ElementEndpoint<ResourceRevision>
    {
        public ResourceRevisionEndpoint(ResourceRevisionCollectionEndpoint referrer, Uri relativeUri)
            : base(referrer, relativeUri)
        {
        }

        /// <summary>
        /// Represents the blob/file backing the <see cref="ResourceRevision"/>.
        /// </summary>
        public IBlobEndpoint Blob => new BlobEndpoint(this, Link("blob"));

        /// <summary>
        /// Promotes the <see cref="ResourceRevision"/> to the next stage.
        /// </summary>
        public IActionEndpoint Promote => new ActionEndpoint(this, Link("promote"));
    }
}