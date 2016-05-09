using System;
using TypedRest;
using TypedRestSample.Model;

namespace TypedRestSample.Client
{
    /// <summary>
    /// REST endpoint that represents the <see cref="ResourceRevision"/>s of a <see cref="Resource"/>.
    /// </summary>
    public class ResourceRevisionCollectionEndpoint : CollectionEndpointBase<ResourceRevision, ResourceRevisionEndpoint>
    {
        public ResourceRevisionCollectionEndpoint(IEndpoint parent)
            : base(parent, parent.Link("revisions"))
        {
        }

        public override ResourceRevisionEndpoint this[Uri relativeUri] => new ResourceRevisionEndpoint(this, relativeUri);

        /// <summary>
        /// Represents the latest <see cref="ResourceRevision"/> for the <see cref="Resource"/>.
        /// </summary>
        public IElementEndpoint<ResourceRevision> Latest => new ElementEndpoint<ResourceRevision>(this, Link("latest"));
    }
}