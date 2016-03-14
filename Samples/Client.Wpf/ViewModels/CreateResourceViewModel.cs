using Caliburn.Micro;
using TypedRest.Samples.Model;
using TypedRest.Wpf.ViewModels;

namespace TypedRest.Samples.Client.Wpf.ViewModels
{
    public class CreateResourceViewModel : CreateElementViewModel<Resource, ResourceEndpoint>
    {
        public CreateResourceViewModel(ICollectionEndpoint<Resource, ResourceEndpoint> endpoint, IEventAggregator eventAggregator)
            : base(endpoint, eventAggregator)
        {
        }
    }
}