using System;
using TypedRest;
using TypedRestSample.Model;

namespace TypedRestSample.Client
{
    /// <summary>
    /// REST endpoint that represents the set of <see cref="Resource"/>s.
    /// </summary>
    public class ResourceCollectionEndpoint : CollectionEndpointBase<Resource, ResourceEndpoint>
    {
        public ResourceCollectionEndpoint(IEndpoint parent)
            : base(parent, parent.Link("resources"))
        {
        }

        public override ResourceEndpoint this[Uri relativeUri] => new ResourceEndpoint(this, relativeUri);
    }
}