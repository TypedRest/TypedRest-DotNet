using System;
using Caliburn.Micro;
using TypedRest.Samples.Library.Endpoints;
using TypedRest.Samples.Library.Models;
using TypedRest.Wpf.ViewModels;

namespace TypedRest.Samples.Wpf.ViewModels
{
    public class ShellViewModel : Conductor<IScreen>.Collection.OneActive
    {
        private SampleEntryEndpoint _endpoint;

        protected override void OnInitialize()
        {
            _endpoint = new SampleEntryEndpoint(new Uri("http://localhost:8080/"));

            base.OnInitialize();
        }

        public void ListResources()
        {
            ActivateItem(new ResourceCollectionViewModel(_endpoint.Resources));
        }

        public void ListTargets()
        {
            ActivateItem(new CollectionViewModel<Target>(_endpoint.Targets) {DisplayName = "Targets"});
        }
    }
}