# ![TypedRest](https://raw.githubusercontent.com/TypedRest/TypedRest-DotNet/master/logo.svg) for .NET

TypedRest helps you build type-safe, fluent-style REST API clients. Common REST patterns such as collections are represented as classes, allowing you to write more idiomatic code.

    dotnet add package TypedRest

Describe your API by deriving from `EntryEndpoint` and exposing its resources as properties:

```csharp
class MyClient(Uri uri) : EntryEndpoint(uri)
{
    public CollectionEndpoint<Contact> Contacts => new(this, relativeUri: "./contacts");
}
```

The endpoint types then provide the methods that match each resource:

```csharp
var client = new MyClient(new Uri("http://example.com/"));

// GET /contacts
List<Contact> contactList = await client.Contacts.ReadAllAsync();

// POST /contacts -> Location: /contacts/1337
ContactEndpoint smith = await client.Contacts.CreateAsync(new Contact {Name = "Smith"});
//ContactEndpoint smith = client.Contacts["1337"];

// GET /contacts/1337
Contact contact = await smith.ReadAsync();

// PUT /contacts/1337/note
await smith.Note.SetAsync(new Note {Content = "some note"});

// DELETE /contacts/1337
await smith.DeleteAsync();
```

Besides collections and elements there are endpoint types for RPC-style calls, binary blobs and uploads, and (with [TypedRest.Reactive](https://www.nuget.org/packages/TypedRest.Reactive/)) push streams. See the [endpoint types](https://typedrest.net/endpoints/) for the full list.

## Dependency injection

In an ASP.NET Core service (or anything else using `IServiceProvider`), register your client with `.AddTypedRest<MyClient>(uri)`. This wires it up with [HttpClientFactory](https://learn.microsoft.com/en-us/dotnet/architecture/microservices/implement-resilient-applications/use-httpclientfactory-to-implement-resilient-http-requests) and returns an `IHttpClientBuilder` for further configuration, e.g. `.AddBasicAuth(…)` or [`.AddOAuthHandler(…)`](https://www.nuget.org/packages/TypedRest.OAuth/).

## Related packages

- [TypedRest.Reactive](https://www.nuget.org/packages/TypedRest.Reactive/) adds support for streaming with ReactiveX (Rx).
- [TypedRest.SystemTextJson](https://www.nuget.org/packages/TypedRest.SystemTextJson/) adds support for serializing with System.Text.Json instead of Newtonsoft.Json.
- [TypedRest.OAuth](https://www.nuget.org/packages/TypedRest.OAuth/) adds OAuth 2.0 / OpenID Connect authentication to HttpClient.
- [TypedRest.CommandLine](https://www.nuget.org/packages/TypedRest.CommandLine/) builds command-line interfaces for TypedRest clients.

## Links

- [Introduction](https://typedrest.net/introduction/)
- [Setup guide](https://typedrest.net/setup/dotnet/)
- [API documentation](https://dotnet.typedrest.net/)
- [Sample project](https://github.com/TypedRest/Sample-DotNet)
