using System;
using System.Net;
using Caliburn.Micro;
using TypedRest.Samples.Library.Endpoints;
using TypedRest.Samples.Library.Models;
using TypedRest.Wpf.ViewModels;

namespace TypedRest.Samples.Wpf.ViewModels
{
    public class ShellViewModel : Conductor<IScreen>.Collection.OneActive
    {
        protected SampleEntryEndpoint Endpoint;

        protected override void OnInitialize()
        {
            Endpoint = new SampleEntryEndpoint(new Uri("http://localhost:5893/api"),
                new NetworkCredential("webconsole", "abc"));

            base.OnInitialize();
        }

        public void AddTestData()
        {
            ActivateItem(new TriggerViewModel(Endpoint.TestData, caption: "Test data"));
        }

        public void ListResources()
        {
            ActivateItem(new ResourceCollectionViewModel(Endpoint.Resources));
        }

        public void ListResourcesPaged()
        {
            ActivateItem(new PagedResourceCollectionViewModel(Endpoint.ResourcesPaged));
        }

        public void ListTargets()
        {
            ActivateItem(new CollectionViewModel<Target>(Endpoint.Targets));
        }
    }
}