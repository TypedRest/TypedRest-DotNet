# TypedRest for .NET

[![Build status](https://img.shields.io/appveyor/ci/TypedRest/typedrest-dotnet.svg)](https://ci.appveyor.com/project/TypedRest/typedrest-dotnet)  
TypedRest helps you build type-safe, fluent-style REST API clients.

Common REST patterns such as collections are represented as classes, allowing you to write more idiomatic code. For example, TypedRest lets you turn this:

```csharp
var httpClient = new HttpClient {BaseAddress = new Uri("http://example.com/")};
var response = httpClient.GetAsync("/contacts/123");
var contact = await response.Content.ReadAsAsync<Contact>();
```

into this:

```csharp
var myService = new MyServiceClient(new Uri("http://example.com/"));
var contact = myService.Contacts["123"].ReadAsync();
```

## NuGet packages

[![TypedRest](https://img.shields.io/nuget/v/TypedRest.svg?label=TypedRest)](https://www.nuget.org/packages/TypedRest/)  
The main TypedRest library.

[![TypedRest.Reactive](https://img.shields.io/nuget/v/TypedRest.Reactive.svg?label=TypedRest.Reactive)](https://www.nuget.org/packages/TypedRest.Reactive/)  
Adds support for streaming with [ReactiveX (Rx)](http://reactivex.io/) to TypedRest.

[![TypedRest.JsonPatch](https://img.shields.io/nuget/v/TypedRest.JsonPatch.svg?label=TypedRest.JsonPatch)](https://www.nuget.org/packages/TypedRest.JsonPatch/)  
Adds support for [JSON Patch](http://jsonpatch.com/) to TypedRest.

[![TypedRest.OAuth](https://img.shields.io/nuget/v/TypedRest.OAuth.svg?label=TypedRest.OAuth)](https://www.nuget.org/packages/TypedRest.OAuth/)  
Adds support for [OAuth 2.0](https://oauth.net/2/) / [OpenID Connect](https://openid.net/connect/) authentication to TypedRest.

[![TypedRest.CommandLine](https://img.shields.io/nuget/v/TypedRest.CommandLine.svg?label=TypedRest.CommandLine)](https://www.nuget.org/packages/TypedRest.CommandLine/)  
Build command-line interfaces for TypedRest clients.

[![TypedRest.Wpf](https://img.shields.io/nuget/v/TypedRest.Wpf.svg?label=TypedRest.Wpf)](https://www.nuget.org/packages/TypedRest.Wpf/)  
Build WPF interfaces for TypedRest clients.

## Documentation

Read an **[Introduction](https://typedrest.net/introduction/)** to TypedRest or jump right in with the **[Getting started](https://typedrest.net/getting-started/dotnet/)** guide.

For information about specific classes or interfaces you can read the **[API Documentation](https://dotnet.typedrest.net/)**.
