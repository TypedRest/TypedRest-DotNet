using System;
using System.Configuration;
using System.Threading.Tasks;
using XProjectNamespaceX.Model;
using Nito.AsyncEx;
using TypedRest.CommandLine;

namespace XProjectNamespaceX.Client.CommandLine
{
    public static class Program
    {
        public static int Main(string[] args) => AsyncContext.Run(() => MainAsync(args));

        private static async Task<int> MainAsync(string[] args)
        {
            var endpoint = new MyEntryEndpoint(
                new Uri(ConfigurationManager.ConnectionStrings["XProjectNamespaceX"].ConnectionString));
            var command = new EntryCommand<MyEntryEndpoint>(endpoint)
            {
                {"entities", x => new CollectionCommand<MyEntity>(x.Entities)}
            };
            return await Executor.RunAsync(command, args);
        }
    }
}
