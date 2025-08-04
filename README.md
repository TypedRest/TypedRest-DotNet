# ![TypedRest](https://raw.githubusercontent.com/TypedRest/TypedRest-DotNet/master/logo.svg) for .NET

[![Build](https://github.com/TypedRest/TypedRest-DotNet/actions/workflows/build.yml/badge.svg)](https://github.com/TypedRest/TypedRest-DotNet/actions/workflows/build.yml)
[![API documentation](https://img.shields.io/badge/api-docs-orange.svg)](https://dotnet.typedrest.net/)  
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

You can also take a look at our [Sample project](https://github.com/TypedRest/Sample-DotNet).

## NuGet packages

[![TypedRest](https://img.shields.io/nuget/v/TypedRest.svg?label=TypedRest)](https://www.nuget.org/packages/TypedRest/)  
The main TypedRest library.

[![TypedRest.Reactive](https://img.shields.io/nuget/v/TypedRest.Reactive.svg?label=TypedRest.Reactive)](https://www.nuget.org/packages/TypedRest.Reactive/)  
Adds support for streaming with [ReactiveX (Rx)](http://reactivex.io/).  
Create endpoints using the types in the `TypedRest.Endpoints.Reactive` namespace.

[![TypedRest.SystemTextJson](https://img.shields.io/nuget/v/TypedRest.SystemTextJson.svg?label=TypedRest.SystemTextJson)](https://www.nuget.org/packages/TypedRest.Reactive/)  
Adds support for serializing using [System.Text.Json](https://learn.microsoft.com/en-us/dotnet/api/system.text.json) instead of [Newtonsoft.Json](https://www.newtonsoft.com/json).  
Pass `new SystemTextJsonSerializer()` to the `EntryEndpoint` constructor.

[![TypedRest.OAuth](https://img.shields.io/nuget/v/TypedRest.OAuth.svg?label=TypedRest.OAuth)](https://www.nuget.org/packages/TypedRest.OAuth/)  
Adds support for [OAuth 2.0](https://oauth.net/2/) / [OpenID Connect](https://openid.net/connect/) authentication to [HttpClient](https://learn.microsoft.com/en-us/dotnet/api/system.net.http.httpclient).  
Call `.AddOAuthHandler()` after `.AddTypedRest()` (or `.AddHttpClient()` when not using main TypedRest package).

[![TypedRest.CommandLine](https://img.shields.io/nuget/v/TypedRest.CommandLine.svg?label=TypedRest.CommandLine)](https://www.nuget.org/packages/TypedRest.CommandLine/)  
Build command-line interfaces for TypedRest clients.  
Create commands mirroring the endpoints using the types in the `TypedRest.CommandLine.Commands` namespace.

## Building

The source code is in [`src/`](src/), config for building the API documentation is in [`doc/`](doc/) and generated build artifacts are placed in `artifacts/`. The source code does not contain version numbers. Instead the version is determined during CI using [GitVersion](https://gitversion.net/).

To build run `.\build.ps1` or `./build.sh` (.NET SDK is automatically downloaded if missing using [0install](https://0install.net/)).

## Contributing

We welcome contributions to this project such as bug reports, recommendations and pull requests.

This repository contains an [EditorConfig](http://editorconfig.org/) file. Please make sure to use an editor that supports it to ensure consistent code style, file encoding, etc.. For full tooling support for all style and naming conventions consider using JetBrains' [ReSharper](https://www.jetbrains.com/resharper/) or [Rider](https://www.jetbrains.com/rider/) products.
