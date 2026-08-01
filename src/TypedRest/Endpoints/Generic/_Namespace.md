---
uid: TypedRest.Endpoints.Generic
summary: Generic endpoints allow you to model collections and elements.
---
A [collection endpoint](https://typedrest.net/endpoints/generic/collection/) represents a set of resources, and indexing it (or creating an entity in it) yields the [element endpoint](https://typedrest.net/endpoints/generic/element/) for an individual resource. Use an [indexer endpoint](https://typedrest.net/endpoints/generic/indexer/) instead when elements are addressable by ID but the set as a whole cannot be listed or added to.

`UpdateAsync()` combines a read and a write into a read-modify-write cycle. It uses the `ETag` from the read to send a conditional write and retries the whole cycle if the server reports a conflict, giving you optimistic concurrency without writing the retry loop yourself.

Whether the server actually permits an operation is discovered from the `Allow` header of the last response and exposed as nullable `bool` properties such as `CreateAllowed` or `SetAllowed`; `null` means "not known yet". These are handy for enabling or disabling controls in a UI without hard-coding the server's policy.

Each type comes in variants with an extra type parameter for the child endpoint type. Use them to plug in your own element endpoint, e.g. one that exposes further sub-resources:

```csharp
class ContactEndpoint(IEndpoint referrer, Uri relativeUri) : ElementEndpoint<Contact>(referrer, relativeUri)
{
    public ElementEndpoint<Note> Note => new(this, relativeUri: "./note");
}

class MyClient(Uri uri) : EntryEndpoint(uri)
{
    public CollectionEndpoint<Contact, ContactEndpoint> Contacts => new(this, relativeUri: "./contacts");
}
```
