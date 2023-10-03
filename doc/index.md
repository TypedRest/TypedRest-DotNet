---
title: Home
---

# TypedRest for .NET

TypedRest helps you build type-safe, fluent-style REST API clients. Common REST patterns such as collections are represented as classes, allowing you to write more idiomatic code.

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

Read a more detailed **[Introduction](https://typedrest.net/introduction/)** to TypedRest or jump right in with the **[Getting started](https://typedrest.net/getting-started/dotnet/)** guide.

Take a look at the <xref:TypedRest.Endpoints.Generic> namespace to get an overview of the available functionality.

**NuGet packages**

[TypedRest](https://www.nuget.org/packages/TypedRest/)  
The main TypedRest library.

[TypedRest.Reactive](https://www.nuget.org/packages/TypedRest.Reactive/)  
Adds support for streaming with [ReactiveX (Rx)](http://reactivex.io/).  
Create endpoints using the types in the <xref:TypedRest.Endpoints.Reactive> namespace.

[TypedRest.OAuth](https://www.nuget.org/packages/TypedRest.OAuth/)  
Adds support for [OAuth 2.0](https://oauth.net/2/) / [OpenID Connect](https://openid.net/connect/) authentication to <xref:System.Net.Http.HttpClient>.  
Call `.AddOAuthHandler()` after `.AddTypedRest()` (or `.AddHttpClient()` when not using main TypedRest package).

[TypedRest.CommandLine](https://www.nuget.org/packages/TypedRest.CommandLine/)  
Build command-line interfaces for TypedRest clients.  
Create commands mirroring the endpoints using the types in the <xref:TypedRest.CommandLine.Commands> namespace.
