using Caliburn.Micro;
using TypedRest.Samples.Model;
using TypedRest.Wpf.ViewModels;

namespace TypedRest.Samples.Client.Wpf.ViewModels
{
    public class ResourceRevisionViewModel : ElementViewModel<ResourceRevision>
    {
        protected new readonly ResourceRevisionEndpoint Endpoint;

        public ResourceRevisionViewModel(ResourceRevisionEndpoint endpoint, IEventAggregator eventAggregator)
            : base(endpoint, eventAggregator)
        {
            Endpoint = endpoint;
        }
    }
}