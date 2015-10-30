using TypedRest.Samples.Library.Endpoints;
using TypedRest.Samples.Library.Models;
using TypedRest.Wpf.ViewModels;

namespace TypedRest.Samples.Wpf.ViewModels
{
    public class UpdateResourceRevisionElementViewModel : UpdateElementViewModel<ResourceRevision>
    {
        public UpdateResourceRevisionElementViewModel(ResourceRevisionElement endpoint) : base(endpoint)
        {
        }
    }
}