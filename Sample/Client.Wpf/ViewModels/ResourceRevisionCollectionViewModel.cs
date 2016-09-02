using Caliburn.Micro;
using TypedRest.Wpf.ViewModels;
using TypedRestSample.Model;

namespace TypedRestSample.Client.Wpf.ViewModels
{
    public class ResourceRevisionCollectionViewModel : CollectionViewModelBase<ResourceRevision, ResourceRevisionCollectionEndpoint, ResourceRevisionEndpoint>
    {
        public ResourceRevisionCollectionViewModel(ResourceRevisionCollectionEndpoint endpoint, IEventAggregator eventAggregator)
            : base(endpoint, eventAggregator)
        {
            DisplayName = "Revisions";
        }

        protected override IScreen BuildElementScreen(ResourceRevisionEndpoint elementEndpoint) => new ResourceRevisionViewModel(elementEndpoint, EventAggregator);

        protected override IScreen BuildCreateElementScreen() => new CreateResourceRevisionViewModel(Endpoint, EventAggregator);
    }
}