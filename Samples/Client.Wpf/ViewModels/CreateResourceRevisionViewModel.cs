using Caliburn.Micro;
using TypedRest.Samples.Model;
using TypedRest.Wpf.ViewModels;

namespace TypedRest.Samples.Client.Wpf.ViewModels
{
    public class CreateResourceRevisionViewModel : CreateElementViewModel<ResourceRevision, ResourceRevisionEndpoint>
    {
        public CreateResourceRevisionViewModel(ICollectionEndpoint<ResourceRevision, ResourceRevisionEndpoint> endpoint, IEventAggregator eventAggregator)
            : base(endpoint, eventAggregator)
        {
        }
    }
}