using TypedRest.CommandLine;
using TypedRest.Samples.Model;

namespace TypedRest.Samples.Client.CommandLine
{
    public class ResourceCollectionCommand : CollectionCommandBase<Resource, ResourceCollection, ResourceElement>
    {
        public ResourceCollectionCommand(ResourceCollection endpoint) : base(endpoint)
        {
        }

        protected override IEndpointCommand GetElementCommand(ResourceElement element)
        {
            return new ResourceElementCommand(element);
        }
    }
}