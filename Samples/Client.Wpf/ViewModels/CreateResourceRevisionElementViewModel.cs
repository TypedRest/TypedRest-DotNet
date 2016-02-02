using Caliburn.Micro;
using TypedRest.Samples.Model;
using TypedRest.Wpf.ViewModels;

namespace TypedRest.Samples.Client.Wpf.ViewModels
{
    public class CreateResourceRevisionElementViewModel : CreateElementViewModel<ResourceRevision, ResourceRevisionElement>
    {
        public CreateResourceRevisionElementViewModel(ICollectionEndpoint<ResourceRevision, ResourceRevisionElement> endpoint, IEventAggregator eventAggregator)
            : base(endpoint, eventAggregator)
        {
        }
    }
}