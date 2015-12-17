using System;
using Nito.AsyncEx;
using TypedRest.CommandLine;
using TypedRest.Samples.Library.Endpoints;
using TypedRest.Samples.Library.Models;

namespace TypedRest.Samples.CommandLine
{
    public static class Program
    {
        public static int Main(string[] args)
        {
            var endpoint = new SampleEntryEndpoint(new Uri("http://localhost:8080/"));
            var command = new EntryCommand<SampleEntryEndpoint>(endpoint)
            {
                {"resources", x => new ResourceCollectionCommand(x.Resources)},
                {"targets", x => new CollectionCommand<Target>(x.Targets)}
            };

            return AsyncContext.Run(() => Executor.RunAsync(command, args));
        }
    }
}