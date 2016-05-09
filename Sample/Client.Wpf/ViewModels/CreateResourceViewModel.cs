using Caliburn.Micro;
using TypedRest;
using TypedRest.Wpf.ViewModels;
using TypedRestSample.Model;

namespace TypedRestSample.Client.Wpf.ViewModels
{
    public class CreateResourceViewModel : CreateElementViewModel<Resource, ResourceEndpoint>
    {
        public CreateResourceViewModel(ICollectionEndpoint<Resource, ResourceEndpoint> endpoint, IEventAggregator eventAggregator)
            : base(endpoint, eventAggregator)
        {
        }
    }
}