﻿using System;
using System.Configuration;
using System.Net;
using Caliburn.Micro;
using TypedRest.Samples.Library.Endpoints;

namespace TypedRest.Samples.Wpf.ViewModels
{
    public class ShellViewModel : Conductor<IScreen>.Collection.OneActive
    {
        protected override void OnActivate()
        {
            DisplayName = "TypedRest WPF Sample";

            base.OnActivate();

            EnsureItem(new SampleEntryViewModel(new SampleEntryEndpoint(
                uri: new Uri(ConfigurationManager.AppSettings["SampleUri"]),
                credentials: new NetworkCredential(
                    ConfigurationManager.AppSettings["SampleUsername"],
                    ConfigurationManager.AppSettings["SamplePassword"]))));
        }
    }
}