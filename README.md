# TypedRest

TypedRest helps you build type-safe fluent-style REST API clients.

NuGet packages (for .NET Framework 4.5+ and .NET Standard 2.0+):  
[![TypedRest](https://img.shields.io/nuget/v/TypedRest.svg?label=TypedRest)](https://www.nuget.org/packages/TypedRest/)
[![TypedRest.CommandLine](https://img.shields.io/nuget/v/TypedRest.CommandLine.svg?label=TypedRest.CommandLine)](https://www.nuget.org/packages/TypedRest.CommandLine/)
[![TypedRest.Wpf](https://img.shields.io/nuget/v/TypedRest.Wpf.svg?label=TypedRest.Wpf)](https://www.nuget.org/packages/TypedRest.Wpf/)

CI Builds:  
[![Windows](https://img.shields.io/appveyor/ci/TypedRest/typedrest-dotnet.svg?label=Windows)](https://ci.appveyor.com/project/TypedRest/typedrest-dotnet)
[![Linux](https://img.shields.io/travis/TypedRest/TypedRest-DotNet.svg?label=Linux)](https://travis-ci.org/TypedRest/TypedRest-DotNet)


## Nomenclature

We use the following terms in the library and documentation:
* An __entity__ is a data transfer object that can be serialized (usually as JSON).
* An __endpoint__ is a REST resource at a specific URI.
* An __entry endpoint__ is an _endpoint_ that is the top-level URI of a REST interface.
* An __element endpoint__ is an _endpoint_ that represents a single _entity_.
* A __collection endpoint__ is an _endpoint_ that represents a collection of _entities_ and provides an _element endpoint_ for each of them.
* A __trigger endpoint__ is an _endpoint_ that represents an RPC call to trigger a single action (intentionally un-RESTful).


## Usecase sample

We'll use this simple POCO (Plain old CLR object) class modeling software packages as a sample _entity_ type:
```cs
class Package
{
  [Key]
  public long Id { get; set; }

  public string Name { get; set; }
}
```


## Getting started

Install the [TypedRest](https://www.nuget.org/packages/TypedRest/) NuGet package in your REST client project.

You can then use the classes `EntryEndpoint`, `CollectionEndpoint`, `ElementEndpoint`, `PollingEndpoint`, `ActionEndpoint`, `PaginationEndpoint`, `StreamEndpoint` and `BlobEndpoint` to build a local representation of a remote REST service. Based on our usecase sample this could look like this:
```cs
class SampleEntryEndpoint : EntryEndpoint
{
  public SampleEntryEndpoint(Uri uri) : base(uri)
  {}

  public ICollectionEndpoint<Package> Packages => new CollectionEndpoint<Package>(this, relativeUri: "packages");
}
```

You can then perform CRUD operations like this:
```cs
var server = new SampleEntryEndpoint(new Uri("http://myservice/api/"));
var packages = server.Packages.ReadAllAsync();
var element = await server.Packages.CreateAsync(new Package {...});
var package = await server.Packages[1].ReadAsync();
await server.Packages[1].UpdateAsync(package);
await server.Packages[1].DeleteAsync();
```


## Build command-line clients

Install the [TypedRest.CommandLine](https://www.nuget.org/packages/TypedRest.CommandLine/) NuGet package in your command-line project. You can then use the classes ``CollectionCommand`, `ElementCommand`, `PollingCommand`, `ActionCommand`, `PaginationCommand`, `StreamCommand` and `BlobCommand` to build command objects that parse arguments and operate on `CollectionEndpoint`s, `ElementEndpoint`s, `PollingEndpoint`s, `ActionEndpoint`s, `PaginationEndpoint`s, `StreamEndpoint`s and `BlobEndpoint`s.


## Build GUI clients

Install the [TypedRest.Wpf](https://www.nuget.org/packages/TypedRest.Wpf/) NuGet package to build GUIs with WPF and [Caliburn.Micro](http://caliburnmicro.com/). Derive from `Bootstrapper<TRootView>` and add this class to your `<Application.Resources>` in `App.xaml`.

You can then use the classes `CollectionViewModel`, `ElementViewModel`, `PollingViewModel`, `ActionViewModel`, `PaginationViewModel`, `StreamViewModel` and `BlobViewModel` to operate on `CollectionEndpoint`s, `ElementEndpoint`s, `ActionEndpoint`s, `PaginationEndpoint`s, `StreamEndpoint`s and `BlobEndpoint`s.
