using TypedRest.Samples.Library.Models;

namespace TypedRest.Samples.Library.Endpoints
{
    /// <summary>
    /// REST endpoint that represents the <see cref="ResourceRevision"/>s of a <see cref="Resource"/>.
    /// </summary>
    public class ResourceRevisionCollection : CollectionEndpointBase<ResourceRevision, ResourceRevisionElement>
    {
        /// <summary>
        /// Represents the latest <see cref="ResourceRevision"/> for the <see cref="Resource"/>.
        /// </summary>
        public ElementEndpoint<ResourceRevision> Latest => new ElementEndpoint<ResourceRevision>(this, relativeUri: "latest");

        public ResourceRevisionCollection(ResourceElement parent) : base(parent, relativeUri: "revisions")
        {
        }

        public override ResourceRevisionElement this[string key] => new ResourceRevisionElement(this, relativeUri: key);
    }
}