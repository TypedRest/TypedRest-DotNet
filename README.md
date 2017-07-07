# TypedRest

TypedRest helps you build type-safe fluent-style JSON REST API clients.

NuGet packages:
* [TypedRest](https://www.nuget.org/packages/TypedRest/)
* [TypedRest.CommandLine](https://www.nuget.org/packages/TypedRest.CommandLine/)
* [TypedRest.Wpf](https://www.nuget.org/packages/TypedRest.Wpf/)

The NuGet packages contain binaries for .NET 4.5 and newer as well as .NET Core 1.0 and newer.

**Important information for .NET Core users**

The .NET Core version of TypedRest currently depends on a pre-release version of the `Microsoft.AspNet.WebApi.Client` package. In order to be able to download it you must run:

    nuget sources Add -Name aspnetwebstack-dev -Source https://dotnet.myget.org/F/aspnetwebstack-dev/api/v3/index.json

Alternatively you can copy our [NuGet.config](https://github.com/TypedRest/TypedRest-DotNet/blob/master/NuGet.Config) next to your `.sln` file.

## Nomenclature

We use the following terms in the library and documentation:
* An __entity__ is a data transfer object that can be serialized as JSON.
* An __endpoint__ is a REST resource at a specific URI.
* An __entry endpoint__ is an _endpoint_ that is the top-level URI of a REST interface.
* An __element endpoint__ is an _endpoint_ that represents a single _entity_.
* A __collection endpoint__ is an _endpoint_ that represents a collection of _entities_ and provides an _element endpoint_ for each of them.
* A __trigger endpoint__ is an _endpoint_ that represents an RPC call to trigger a single action (intentionally un-RESTful).


## Usecase sample

We'll use this simple POCO (Plain old CLR object) class modelling software packages as a sample _entity_ type:
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


## Sample projects

The source code includes sample projects.

`Samples\Model` contains a simple JSON-serializable sample model.

`Samples\Client` demonstrates how to create a client library for a REST interface using `TypedRest`.

`Samples\Client.CommandLine` demonstrates how to use such a client library and `TypedRest.CommmandLine` to create a command-line client for a REST interface.

`Samples\Client.Wpf` demonstrates how to use such a client library and `TypedRest.Wpf` to create a GUI client for a REST interface.

`Samples\Server` provides a simple REST API that the acutal TypedRest samples use to have something to communicate with. This does not mean that TypedRest requires a specific type of server! It can interoperate with anything provding a RESTful API.
