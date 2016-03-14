using Caliburn.Micro;
using TypedRest.Samples.Model;
using TypedRest.Wpf.ViewModels;

namespace TypedRest.Samples.Client.Wpf.ViewModels
{
    public class ResourceRevisionCollectionViewModel : CollectionViewModelBase<ResourceRevision, ResourceRevisionCollectionEndpoint, ResourceRevisionEndpoint>
    {
        public ResourceRevisionCollectionViewModel(ResourceRevisionCollectionEndpoint endpoint, IEventAggregator eventAggregator)
            : base(endpoint, eventAggregator)
        {
            DisplayName = "Revisions";
        }

        protected override IScreen BuildElementScreen(ResourceRevisionEndpoint elementEndpoint)
        {
            return new ResourceRevisionViewModel(elementEndpoint, EventAggregator);
        }

        protected override IScreen BuildCreateElementScreen()
        {
            return new CreateResourceRevisionViewModel(Endpoint, EventAggregator);
        }
    }
}