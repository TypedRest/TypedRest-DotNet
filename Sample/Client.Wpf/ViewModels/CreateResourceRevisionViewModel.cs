using Caliburn.Micro;
using TypedRest;
using TypedRest.Wpf.ViewModels;
using TypedRestSample.Model;

namespace TypedRestSample.Client.Wpf.ViewModels
{
    public class CreateResourceRevisionViewModel : CreateElementViewModel<ResourceRevision, ResourceRevisionEndpoint>
    {
        public CreateResourceRevisionViewModel(ICollectionEndpoint<ResourceRevision, ResourceRevisionEndpoint> endpoint, IEventAggregator eventAggregator)
            : base(endpoint, eventAggregator)
        {
        }
    }
}