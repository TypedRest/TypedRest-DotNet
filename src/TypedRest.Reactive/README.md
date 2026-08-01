# ![TypedRest](https://raw.githubusercontent.com/TypedRest/TypedRest-DotNet/master/logo.svg) Reactive

Adds support for streaming with [ReactiveX (Rx)](http://reactivex.io/) to [TypedRest](https://www.nuget.org/packages/TypedRest/).

    dotnet add package TypedRest.Reactive

Create endpoints using the types in the `TypedRest.Endpoints.Reactive` namespace to receive data as push streams rather than pulling it explicitly:

```csharp
var messages = new StreamingCollectionEndpoint<Message>(client, "messages");

// Subscribe to new messages
IObservable<Message> stream = messages.GetObservable();
stream.Subscribe(m => Console.WriteLine($"New message: {m.Text}"));

// Still usable as a regular collection endpoint
List<Message> all = await messages.ReadAllAsync();
await messages.CreateAsync(new Message {Text = "Hello!"});
```

Three endpoint types are available:

- [Polling endpoint](https://typedrest.net/endpoints/reactive/polling/) — polls a resource for state changes
- [Streaming endpoint](https://typedrest.net/endpoints/reactive/streaming/) — stream of entities via a persistent connection
- [Streaming Collection endpoint](https://typedrest.net/endpoints/reactive/streaming-collection/) — collection observable as an append-only stream using long-polling

## Links

- [TypedRest documentation](https://typedrest.net/)
- [API documentation](https://dotnet.typedrest.net/)
