using Caliburn.Micro;
using TypedRest.Samples.Model;
using TypedRest.Wpf.ViewModels;

namespace TypedRest.Samples.Client.Wpf.ViewModels
{
    public class CreateResourceElementViewModel : CreateElementViewModel<Resource, ResourceElement>
    {
        public CreateResourceElementViewModel(ICollectionEndpoint<Resource, ResourceElement> endpoint, IEventAggregator eventAggregator)
            : base(endpoint, eventAggregator)
        {
        }
    }
}