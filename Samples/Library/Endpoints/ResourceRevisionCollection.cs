using System;
using TypedRest.Samples.Library.Models;

namespace TypedRest.Samples.Library.Endpoints
{
    /// <summary>
    /// REST endpoint that represents the <see cref="ResourceRevision"/>s of a <see cref="Resource"/>.
    /// </summary>
    public class ResourceRevisionCollection : CollectionEndpointBase<ResourceRevision, ResourceRevisionElement>
    {
        public ResourceRevisionCollection(IEndpoint parent)
            : base(parent, parent.Link("revisions"))
        {
        }

        public override ResourceRevisionElement this[Uri relativeUri] => new ResourceRevisionElement(this, relativeUri);

        /// <summary>
        /// Represents the latest <see cref="ResourceRevision"/> for the <see cref="Resource"/>.
        /// </summary>
        public ElementEndpoint<ResourceRevision> Latest => new ElementEndpoint<ResourceRevision>(this, Link("latest"));
    }
}