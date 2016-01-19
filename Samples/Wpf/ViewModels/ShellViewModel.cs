using System;
using System.Configuration;
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

            var endpoint = new SampleEntryEndpoint(
                uri: new Uri(ConfigurationManager.AppSettings["SampleUri"]),
                credentials:
                    new NetworkCredential(
                        ConfigurationManager.AppSettings["SampleUsername"],
                        ConfigurationManager.AppSettings["SamplePassword"]));
            var eventAggregator = new EventAggregator();

            EnsureItem(new ResourceCollectionViewModel(endpoint.Resources, eventAggregator));
            EnsureItem(new CollectionViewModel<Target>(endpoint.Targets, eventAggregator));
        }
    }
}