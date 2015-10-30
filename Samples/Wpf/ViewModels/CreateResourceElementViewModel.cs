using TypedRest.Samples.Library.Endpoints;
using TypedRest.Samples.Library.Models;
using TypedRest.Wpf.ViewModels;

namespace TypedRest.Samples.Wpf.ViewModels
{
    public class CreateResourceElementViewModel : CreateElementViewModel<Resource, ResourceElement>
    {
        public CreateResourceElementViewModel(ResourceCollection endpoint) : base(endpoint)
        {
        }
    }
}