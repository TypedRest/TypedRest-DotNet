using Caliburn.Micro;
using TypedRest.Samples.Model;
using TypedRest.Wpf.ViewModels;

namespace TypedRest.Samples.Client.Wpf.ViewModels
{
    public class ResourceCollectionViewModel : CollectionViewModelBase<Resource, ResourceCollectionEndpoint, ResourceEndpoint>
    {
        public ResourceCollectionViewModel(ResourceCollectionEndpoint endpoint, IEventAggregator eventAggregator)
            : base(endpoint, eventAggregator)
        {
            DisplayName = "Resources";
        }

        protected override IScreen BuildElementScreen(ResourceEndpoint elementEndpoint)
        {
            return new ResourceViewModel(elementEndpoint, EventAggregator);
        }

        protected override IScreen BuildCreateElementScreen()
        {
            return new CreateResourceViewModel(Endpoint, EventAggregator);
        }
    }
}