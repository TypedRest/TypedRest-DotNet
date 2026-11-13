---
title: Home
---

# TypedRest for .NET

TypedRest helps you build type-safe, fluent-style REST API clients. Common REST patterns are represented as classes that you compose to mirror the URL structure of your API.

Tell TypedRest that `/contacts` is a collection and get type-safe listing, creating, reading, updating and deleting in return.

## Define your client

A few lines are enough to describe an API with a collection of contacts plus a "notes" endpoint nested underneath. Derive your client from <xref:TypedRest.Endpoints.EntryEndpoint> and compose it from endpoint types such as <xref:TypedRest.Endpoints.Generic.CollectionEndpoint%602> and <xref:TypedRest.Endpoints.Generic.ElementEndpoint%601>:

```csharp
class MyClient(Uri uri) : EntryEndpoint(uri)
{
  public ContactCollectionEndpoint Contacts => new(this);
}

class ContactCollectionEndpoint(IEndpoint referrer)
  : CollectionEndpoint<Contact, ContactEndpoint>(referrer, relativeUri: "./contacts");

class ContactEndpoint(IEndpoint referrer, Uri relativeUri)
  : ElementEndpoint<Contact>(referrer, relativeUri)
{
  public ElementEndpoint<Note> Note => new(this, relativeUri: "./note");
}
```

## Use it

Navigate the API like an object model, with code completion guiding the way:

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

// GET /contacts/1337/note
Note note = await smith.Note.ReadAsync();

// DELETE /contacts/1337
await smith.DeleteAsync();
```

## Learn more

Read a more detailed **[Introduction](https://typedrest.net/introduction/)** to TypedRest or jump right in with the **[setup guide](https://typedrest.net/setup/dotnet/)**.  
You can also **[build your first client](https://typedrest.net/tutorials/first-client/)** step by step or **[generate one from OpenAPI](https://typedrest.net/tutorials/from-openapi/)**.

Take a look at the <xref:TypedRest.Endpoints.Generic> namespace to get an overview of the available functionality.

## NuGet packages

[TypedRest](https://www.nuget.org/packages/TypedRest/)  
The main TypedRest library.

[TypedRest.Reactive](https://www.nuget.org/packages/TypedRest.Reactive/)  
Adds support for streaming with [ReactiveX (Rx)](http://reactivex.io/).  
Create endpoints using the types in the <xref:TypedRest.Endpoints.Reactive> namespace.

[TypedRest.SystemTextJson](https://www.nuget.org/packages/TypedRest.SystemTextJson/)  
Adds support for serializing using [System.Text.Json](https://learn.microsoft.com/en-us/dotnet/api/system.text.json) instead of [Newtonsoft.Json](https://www.newtonsoft.com/json).  
Pass a <xref:TypedRest.Serializers.SystemTextJsonSerializer> instance to the <xref:TypedRest.Endpoints.EntryEndpoint> constructor.

[TypedRest.OAuth](https://www.nuget.org/packages/TypedRest.OAuth/)  
Adds support for [OAuth 2.0](https://oauth.net/2/) / [OpenID Connect](https://openid.net/connect/) authentication to <xref:System.Net.Http.HttpClient>.  
Call `.AddOAuthHandler()` after `.AddTypedRest()` (or `.AddHttpClient()` when not using main TypedRest package).

[TypedRest.CommandLine](https://www.nuget.org/packages/TypedRest.CommandLine/)  
Build command-line interfaces for TypedRest clients.  
Create commands mirroring the endpoints using the types in the <xref:TypedRest.CommandLine.Commands> namespace.
