using Caliburn.Micro;
using TypedRest.Wpf.ViewModels;
using TypedRestSample.Model;

namespace TypedRestSample.Client.Wpf.ViewModels
{
    public class ResourceCollectionViewModel : CollectionViewModelBase<Resource, ResourceCollectionEndpoint, ResourceEndpoint>
    {
        public ResourceCollectionViewModel(ResourceCollectionEndpoint endpoint, IEventAggregator eventAggregator)
            : base(endpoint, eventAggregator)
        {
            DisplayName = "Resources";
        }

        protected override IScreen BuildElementScreen(ResourceEndpoint elementEndpoint) => new ResourceViewModel(elementEndpoint, EventAggregator);

        protected override IScreen BuildCreateElementScreen() => new CreateResourceViewModel(Endpoint, EventAggregator);
    }
}