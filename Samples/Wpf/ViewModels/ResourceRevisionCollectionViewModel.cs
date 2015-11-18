using Caliburn.Micro;
using TypedRest.Samples.Library.Endpoints;
using TypedRest.Samples.Library.Models;
using TypedRest.Wpf.ViewModels;

namespace TypedRest.Samples.Wpf.ViewModels
{
    public class ResourceRevisionCollectionViewModel : CollectionViewModelBase<ResourceRevision, ResourceRevisionCollection, ResourceRevisionElement>
    {
        public ResourceRevisionCollectionViewModel(ResourceRevisionCollection endpoint) : base(endpoint)
        {
            DisplayName = "Revisions";
        }

        protected override IScreen BuildCreateElementScreen()
        {
            return new CreateResourceRevisionElementViewModel(Endpoint);
        }

        protected override IScreen BuildUpdateElementScreen(ResourceRevisionElement elementEndpoint)
        {
            return new UpdateResourceRevisionElementViewModel(elementEndpoint);
        }
    }
}