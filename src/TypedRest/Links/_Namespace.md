---
uid: TypedRest.Links
summary: '[Handling links](https://typedrest.net/link-handling/) between HTTP resources.'
---
Instead of hard-coding every URI into your client, you can follow the links a server advertises. See [Link handling](https://typedrest.net/link-handling/) for the methods all endpoints provide for resolving links and for the supported sources: [relative URIs](https://typedrest.net/link-handling/relative-uris/), [URI templates](https://typedrest.net/link-handling/uri-templates/), [HTTP `Link` headers](https://typedrest.net/link-handling/link-header/) and [HAL](https://typedrest.net/link-handling/hal/).

```csharp
Uri next = endpoint.Link("next");
Uri contact = endpoint.LinkTemplate("contact", new {id = "1337"});
```

Which of those sources are actually consulted is decided by the `ILinkExtractor` passed to the `EntryEndpoint` and inherited by all children. The default combines `HeaderLinkExtractor` and `HalLinkExtractor` via `AggregateLinkExtractor`. Supporting another hypermedia format means implementing `ILinkExtractor` and passing it in.

```csharp
class MyClient(Uri uri) : EntryEndpoint(uri, linkExtractor: new MyLinkExtractor())
{
    // ...
}
```
