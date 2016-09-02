using TypedRest;
using TypedRestSample.Model;

namespace TypedRestSample.Client
{
    /// <summary>
    /// REST endpoint that represents the <see cref="ResourceRevision"/>s of a <see cref="Resource"/>.
    /// </summary>
    public class ResourceRevisionCollectionEndpoint : CollectionEndpoint<ResourceRevision, ResourceRevisionEndpoint>
    {
        public ResourceRevisionCollectionEndpoint(IEndpoint referrer)
            : base(referrer, referrer.Link("revisions"))
        {
        }

        /// <summary>
        /// Represents the latest <see cref="ResourceRevision"/> for the <see cref="Resource"/>.
        /// </summary>
        public IElementEndpoint<ResourceRevision> Latest => new ElementEndpoint<ResourceRevision>(this, Link("latest"));
    }
}