using TypedRest.Wpf.ViewModels;
using TypedRestSample.Model;

namespace TypedRestSample.Client.Wpf.ViewModels
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