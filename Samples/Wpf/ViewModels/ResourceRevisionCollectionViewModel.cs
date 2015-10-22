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
            DisplayName = "Resource revisions";
        }

        protected override IScreen GetElementScreen(ResourceRevisionElement elementEndpoint)
        {
            return new ResourceRevisionElementViewModel(elementEndpoint);
        }
    }
}