using Caliburn.Micro;
using TypedRest.Samples.Model;
using TypedRest.Wpf.ViewModels;

namespace TypedRest.Samples.Client.Wpf.ViewModels
{
    public class ResourceCollectionViewModel : CollectionViewModelBase<Resource, ResourceCollection, ResourceElement>
    {
        public ResourceCollectionViewModel(ResourceCollection endpoint, IEventAggregator eventAggregator)
            : base(endpoint, eventAggregator)
        {
            DisplayName = "Resources";
        }

        protected override IScreen BuildElementScreen(ResourceElement elementEndpoint)
        {
            return new ResourceElementViewModel(elementEndpoint, EventAggregator);
        }

        protected override IScreen BuildCreateElementScreen()
        {
            return new CreateResourceElementViewModel(Endpoint, EventAggregator);
        }
    }
}