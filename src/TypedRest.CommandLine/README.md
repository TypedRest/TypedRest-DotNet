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

`Executor` is the piece you call from `Main()`. It asks an `IEndpointProvider` for the entry endpoint, constructs the root command for it, hands it the arguments and turns any exception thrown along the way into a message on stderr and a non-zero exit code.

By default the entry endpoint is built by `CliEndpointProvider<T>`, which reads the URI and OAuth token from config files or prompts for them interactively. Pass your own `IEndpointProvider<T>` to the `Executor` constructor to change that.

Splitting out the endpoint provider keeps the question of *where the API lives and how to authenticate against it* separate from *what the commands do*. The default implementation stores the API URI and a cached OAuth token in the user's application data directory and prompts for whatever is missing; it only does so when running interactively, so the same binary can be used in scripts. Derive from `EndpointProviderBase` to change where those settings come from, or implement `IEndpointProvider` yourself and pass it to the `Executor` constructor.
