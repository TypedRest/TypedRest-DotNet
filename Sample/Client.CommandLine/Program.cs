using System;
using System.Configuration;
using System.Net;
using Nito.AsyncEx;
using TypedRest.CommandLine;
using TypedRestSample.Model;

namespace TypedRestSample.Client.CommandLine
{
    public static class Program
    {
        public static int Main(string[] args)
        {
            var endpoint = new SampleEntryEndpoint(
                uri: new Uri(ConfigurationManager.AppSettings["SampleUri"]),
                credentials: new NetworkCredential(
                    ConfigurationManager.AppSettings["SampleUsername"],
                    ConfigurationManager.AppSettings["SamplePassword"]));
            var command = new EntryCommand<SampleEntryEndpoint>(endpoint)
            {
                {"resources", x => new CollectionCommand<Resource, ResourceEndpoint, ResourceCommand>(x.Resources)},
                {"targets", x => new CollectionCommand<Target>(x.Targets)}
            };

            return AsyncContext.Run(() => Executor.RunAsync(command, args));
        }
    }
}