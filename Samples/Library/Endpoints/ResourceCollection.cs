using System;
using TypedRest.Samples.Library.Models;

namespace TypedRest.Samples.Library.Endpoints
{
    /// <summary>
    /// REST endpoint that represents the set of <see cref="Resource"/>s.
    /// </summary>
    public class ResourceCollection : CollectionEndpointBase<Resource, ResourceElement>
    {
        public ResourceCollection(SampleEntryEndpoint parent) : base(parent, relativeUri: "resources")
        {
        }

        public override ResourceElement this[Uri relativeUri] => new ResourceElement(this, relativeUri);
    }
}