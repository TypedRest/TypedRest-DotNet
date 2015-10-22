using TypedRest.Samples.Library.Models;
using TypedRest.Wpf.ViewModels;

namespace TypedRest.Samples.Wpf.ViewModels
{
    public class ResourceRevisionElementViewModel : ElementViewModel<ResourceRevision>
    {
        public ResourceRevisionElementViewModel(IElementEndpoint<ResourceRevision> endpoint) : base(endpoint)
        {
        }
    }
}