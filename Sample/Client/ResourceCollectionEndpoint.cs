using TypedRest;
using TypedRestSample.Model;

namespace TypedRestSample.Client
{
    /// <summary>
    /// REST endpoint that represents the set of <see cref="Resource"/>s.
    /// </summary>
    public class ResourceCollectionEndpoint : CollectionEndpoint<Resource, ResourceEndpoint>
    {
        public ResourceCollectionEndpoint(IEndpoint referrer)
            : base(referrer, referrer.Link("resources"))
        {
        }
    }
}