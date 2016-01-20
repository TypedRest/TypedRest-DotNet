using TypedRest.Samples.Library.Endpoints;
using TypedRest.Samples.Library.Models;
using TypedRest.Wpf.ViewModels;

namespace TypedRest.Samples.Wpf.ViewModels
{
    public class SampleEntryViewModel : EntryViewModelBase<SampleEntryEndpoint>
    {
        public SampleEntryViewModel(SampleEntryEndpoint endpoint) : base(endpoint)
        {
            DisplayName = "TypedRest Sample";
        }

        public void OpenResources()
        {
            Open(new ResourceCollectionViewModel(Endpoint.Resources, EventAggregator));
        }

        public void OpenTargets()
        {
            Open(new CollectionViewModel<Target>(Endpoint.Targets, EventAggregator));
        }
    }
}