using TypedRest.Samples.Library.Models;

namespace TypedRest.Samples.Library.Endpoints
{
    /// <summary>
    /// REST endpoint that represents the set of <see cref="Resource"/>s.
    /// </summary>
    public class PagedResourceCollection : PagedCollectionEndpointBase<Resource, ResourceElement>
    {
        public PagedResourceCollection(SampleEntryEndpoint parent) : base(parent, relativeUri: "resources")
        {
        }

        public override ResourceElement this[string key] => new ResourceElement(this, relativeUri: key);
    }
}