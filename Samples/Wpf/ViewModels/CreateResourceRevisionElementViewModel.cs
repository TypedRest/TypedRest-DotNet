using TypedRest.Samples.Library.Endpoints;
using TypedRest.Samples.Library.Models;
using TypedRest.Wpf.ViewModels;

namespace TypedRest.Samples.Wpf.ViewModels
{
    public class CreateResourceRevisionElementViewModel : CreateElementViewModel<ResourceRevision, ResourceRevisionElement>
    {
        public CreateResourceRevisionElementViewModel(ICollectionEndpoint<ResourceRevision, ResourceRevisionElement> endpoint) : base(endpoint)
        {
        }
    }
}