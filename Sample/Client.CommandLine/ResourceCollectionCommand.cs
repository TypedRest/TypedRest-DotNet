using TypedRest.CommandLine;
using TypedRestSample.Model;

namespace TypedRestSample.Client.CommandLine
{
    public class ResourceCollectionCommand : CollectionCommandBase<Resource, ResourceCollectionEndpoint, ResourceEndpoint>
    {
        public ResourceCollectionCommand(ResourceCollectionEndpoint endpoint) : base(endpoint)
        {
        }

        protected override IEndpointCommand GetElementCommand(ResourceEndpoint element)
        {
            return new ResourceCommand(element);
        }
    }
}