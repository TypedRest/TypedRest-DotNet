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
        protected override void OnActivate()
        {
            base.OnActivate();

            var endpoint = new SampleEntryEndpoint(new Uri("http://localhost:5893/api"),
                new NetworkCredential("webconsole", "abc"));
            var eventAggregator = new EventAggregator();

            EnsureItem(new ResourceCollectionViewModel(endpoint.Resources, eventAggregator));
            EnsureItem(new PagedResourceCollectionViewModel(endpoint.ResourcesPaged, eventAggregator));
            EnsureItem(new CollectionViewModel<Target>(endpoint.Targets, eventAggregator));
        }
    }
}