---
uid: TypedRest.Serializers
summary: '[Serialization](https://typedrest.net/serializers/) of entities sent to and received from the server.'
---
Serializers are `MediaTypeFormatter`s. You pass them to the `EntryEndpoint` constructor and all child endpoints inherit them, so the wire format is decided once for the whole client. See [Serializers](https://typedrest.net/serializers/) for the available formats and their default settings, and [Custom serializers](https://typedrest.net/serializers/custom/) for writing your own.

```csharp
class MyClient(Uri uri) : EntryEndpoint(uri, serializer: new XmlSerializer())
{
    // ...
}
```

An endpoint can hold several serializers, sorted from most to least preferred. All of them are announced in the `Accept` header and used to deserialize responses according to their content type, while the first one is always used for serializing requests. This lets a client accept multiple formats from a server while still committing to one for its own payloads.

> [!NOTE]
> `ElementEndpoint.UpdateAsync(Action<JsonPatchDocument<T>>)` builds its patch document with Newtonsoft.Json and thus requires a Newtonsoft.Json-based serializer. The other update overloads work with any serializer.
