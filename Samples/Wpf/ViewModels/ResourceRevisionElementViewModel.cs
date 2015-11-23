using Caliburn.Micro;
using TypedRest.Samples.Library.Endpoints;
using TypedRest.Samples.Library.Models;
using TypedRest.Wpf.ViewModels;

namespace TypedRest.Samples.Wpf.ViewModels
{
    public class ResourceRevisionElementViewModel : ElementViewModel<ResourceRevision>
    {
        protected new readonly ResourceRevisionElement Endpoint;

        public ResourceRevisionElementViewModel(ResourceRevisionElement endpoint) : base(endpoint)
        {
            Endpoint = endpoint;
        }

        public void DoPromote()
        {
            ((IConductor)Parent).ActivateItem(new TriggerViewModel(Endpoint.Promote, caption: "Promote"));
        }
    }
}