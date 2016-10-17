using TypedRest.CommandLine;
using XProjectNamespaceX.Model;

namespace XProjectNamespaceX.Client.CommandLine.Commands
{
    public class MyEntryCommand : EntryCommand<MyEntryEndpoint>
    {
        public MyEntryCommand(MyEntryEndpoint endpoint) : base(endpoint)
        {
            Add("entities", x => new CollectionCommand<MyEntity>(x.Entities));
        }
    }
}