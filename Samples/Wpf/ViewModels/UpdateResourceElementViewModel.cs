using TypedRest.Samples.Library.Endpoints;
using TypedRest.Samples.Library.Models;
using TypedRest.Wpf.ViewModels;

namespace TypedRest.Samples.Wpf.ViewModels
{
    public class UpdateResourceElementViewModel : UpdateElementViewModel<Resource>
    {
        public UpdateResourceElementViewModel(ResourceElement endpoint) : base(endpoint)
        {
        }
    }
}