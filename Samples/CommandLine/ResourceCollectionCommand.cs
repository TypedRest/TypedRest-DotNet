using TypedRest.CommandLine;
using TypedRest.Samples.Library.Endpoints;
using TypedRest.Samples.Library.Models;

namespace TypedRest.Samples.CommandLine
{
    public class ResourceCollectionCommand : CollectionCommandBase<Resource, ResourceCollection, ResourceElement>
    {
        public ResourceCollectionCommand(ResourceCollection endpoint) : base(endpoint)
        {
        }

        protected override ICommand GetElementCommand(ResourceElement element)
        {
            return new ResourceElementCommand(element);
        }
    }
}