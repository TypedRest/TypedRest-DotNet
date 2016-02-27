using System;
using System.Net;
using XProjectNamespaceX.Model;
using Nito.AsyncEx;
using TypedRest.CommandLine;
using XProjectNamespaceX.Client.CommandLine.Properties;

namespace XProjectNamespaceX.Client.CommandLine
{
    public static class Program
    {
        public static int Main(string[] args)
        {
            var endpoint = new MyEntryEndpoint(
                uri: new Uri(Settings.Default.ApiUri),
                credentials: new NetworkCredential(
                    Settings.Default.ApiUsername,
                    Settings.Default.ApiPassword));
            var command = new EntryCommand<MyEntryEndpoint>(endpoint)
            {
                {"entities", x => new CollectionCommand<MyEntity>(x.Entities)}
            };

            return AsyncContext.Run(() => Executor.RunAsync(command, args));
        }
    }
}
