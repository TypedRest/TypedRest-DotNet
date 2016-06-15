using XProjectNamespaceX.Model;
using Nito.AsyncEx;
using TypedRest.CommandLine;

namespace XProjectNamespaceX.Client.CommandLine
{
    public static class Program
    {
        public static int Main(string[] args)
        {
            var endpoint = new MyEntryEndpoint();
            var command = new EntryCommand<MyEntryEndpoint>(endpoint)
            {
                {"entities", x => new CollectionCommand<MyEntity>(x.Entities)}
            };

            return AsyncContext.Run(() => Executor.RunAsync(command, args));
        }
    }
}
