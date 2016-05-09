using Caliburn.Micro;
using TypedRest.Wpf.ViewModels;
using TypedRestSample.Model;

namespace TypedRestSample.Client.Wpf.ViewModels
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