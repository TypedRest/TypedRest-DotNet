# ![TypedRest](https://raw.githubusercontent.com/TypedRest/TypedRest-DotNet/master/logo.svg) CommandLine

Build command-line interfaces for [TypedRest](https://www.nuget.org/packages/TypedRest/) clients.

    dotnet add package TypedRest.CommandLine

Mirror your endpoints with the command types in the `TypedRest.CommandLine.Commands` namespace. Each command type knows how to map console arguments onto the methods of the matching endpoint type, so `myapp contacts create …` ends up calling `client.Contacts.CreateAsync(…)`:

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

Then hand the arguments to an `Executor`, which builds the endpoint and command, runs it and turns exceptions into exit codes:

```csharp
static Task<int> Main(string[] args)
    => new Executor<MyClient, MyCommand>().RunAsync(args);
```

By default the entry endpoint is built by `CliEndpointProvider<T>`, which reads the URI and OAuth token from config files or prompts for them interactively. Pass your own `IEndpointProvider<T>` to the `Executor` constructor to change that.
