---
uid: TypedRest.Endpoints
summary: '[Endpoints](https://typedrest.net/endpoints/) represent URIs that provides methods for operating on specific resources.'
---
Rather than assembling URIs and requests by hand, you compose endpoints into a tree that mirrors the API: an `EntryEndpoint` at the base URI exposes child endpoints as properties, and those may expose children of their own. See [Endpoint types](https://typedrest.net/endpoints/) for the available types and what each child inherits from its referrer.

```csharp
class MyClient(Uri uri) : EntryEndpoint(uri)
{
    public CollectionEndpoint<Contact> Contacts => new(this, relativeUri: "./contacts");
}
```

Endpoints are cheap value-like objects, not connections. Creating one performs no I/O; only calling one of its methods sends a request. That is why they are usually exposed as get-only properties returning a new instance each time.

Write your own endpoint types by deriving from `EndpointBase`, which handles URI resolution, serialization, error handling and link extraction for you.
