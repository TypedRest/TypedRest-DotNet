using Caliburn.Micro;
using TypedRest.Samples.Model;
using TypedRest.Wpf.ViewModels;

namespace TypedRest.Samples.Client.Wpf.ViewModels
{
    public class ResourceElementViewModel : ElementViewModel<Resource>
    {
        protected new readonly ResourceElement Endpoint;

        public ResourceElementViewModel(ResourceElement endpoint, IEventAggregator eventAggregator)
            : base(endpoint, eventAggregator)
        {
            Endpoint = endpoint;
        }

        public void OpenRevisions()
        {
            Open(new ResourceRevisionCollectionViewModel(Endpoint.Revisions, EventAggregator));
        }

        public void OpenEvents()
        {
            Open(new StreamViewModel<LogEvent>(Endpoint.Events, EventAggregator));
        }
    }
}