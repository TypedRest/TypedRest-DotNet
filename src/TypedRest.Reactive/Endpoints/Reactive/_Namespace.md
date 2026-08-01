---
uid: TypedRest.Endpoints.Reactive
summary: Reactive endpoints allow you to receive data as push streams rather than explicitly pulling.
---
> [!NOTE]
> NuGet package: [TypedRest.Reactive](https://www.nuget.org/packages/TypedRest.Reactive/)

All of these endpoints expose their data through a `GetObservable()` method. The observable is cold: no request is sent until something subscribes, and the connection is closed again when the subscription is disposed. The [polling](https://typedrest.net/endpoints/reactive/polling/) and [streaming collection](https://typedrest.net/endpoints/reactive/streaming-collection/) variants additionally derive from their pull-based counterparts in <xref:TypedRest.Endpoints.Generic>, so one instance serves both access patterns; [streaming](https://typedrest.net/endpoints/reactive/streaming/) endpoints only push.

How the data actually gets there stays hidden behind `IObservable<T>`. Polling lets the server dictate the rate via a `Retry-After` header, and an optional end condition completes the stream once the entity reaches its final state. Streaming splits the entities the server writes into the open response body on a configurable separator. A streaming collection uses ranged requests with long polling to wait for new elements.

Where a stream begins matters for the collection variant, since its elements persist: `GetObservable(startIndex)` can replay from the beginning, resume at a known offset, or take a negative index to show the last few entries before following along live.

```csharp
var messages = new StreamingCollectionEndpoint<Message>(client, "./messages");

using var subscription = messages.GetObservable(startIndex: -10)
                                 .Subscribe(m => Console.WriteLine(m.Text));
```
