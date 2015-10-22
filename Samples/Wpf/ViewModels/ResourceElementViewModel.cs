using TypedRest.Samples.Library.Models;
using TypedRest.Wpf.ViewModels;

namespace TypedRest.Samples.Wpf.ViewModels
{
    public class ResourceElementViewModel : ElementViewModel<Resource>
    {
        public ResourceElementViewModel(IElementEndpoint<Resource> endpoint) : base(endpoint)
        {
        }
    }
}