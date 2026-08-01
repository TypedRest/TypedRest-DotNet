---
uid: TypedRest.CommandLine.Commands
summary: Commands operating on <xref:TypedRest.Endpoints>.
---
Commands mirror the structure of your endpoints, so the command-line arguments mirror the code you would otherwise write. Each command wraps one endpoint, consumes the arguments it understands and passes the remainder on to a sub-command. Walking `contacts 1337 note set` down the command tree ends up calling `client.Contacts["1337"].Note.SetAsync(...)`.

```csharp
class MyCommand : EntryCommand<MyClient>
{
    public MyCommand(MyClient client)
        : base(client)
    {
        Add("contacts", x => new CollectionCommand<Contact>(x.Contacts));
    }
}
```

`EntryCommand` is the root of that tree, matching the `EntryEndpoint`. Its `Add()` registers a name and a factory that builds the sub-command from the endpoint; this is evaluated lazily, so nothing is constructed for parts of the API the user did not ask for. The ready-made commands in the sub-namespaces already know the verbs of their endpoint type; for anything else, derive from `EndpointCommand` and override `GetSubCommand()` to route by name and `ExecuteInnerAsync()` to handle the arguments yourself.

Commands never touch `System.Console` directly. They read and write through an <xref:TypedRest.CommandLine.IO.IConsole>, which each command exposes as a settable property. Assigning your own implementation there is what makes a command testable without a terminal.
