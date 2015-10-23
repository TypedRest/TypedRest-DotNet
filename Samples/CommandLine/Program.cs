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
            var entryPoint = new SampleEntryEndpoint(new Uri("http://localhost:8080/"));
            var command = new EntryCommand<SampleEntryEndpoint>(entryPoint)
            {
                {"packages", x => new CollectionCommand<Package>(x.Packages)},
                {"blob", x => new BlobCommand(x.Blob)},
                {"trigger", x => new TriggerCommand(x.Trigger)}
            };

            return AsyncContext.Run(() => Executor.RunAsync(command, args));
        }
    }
}