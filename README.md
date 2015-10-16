# TypedRest

TypedRest helps you build type-safe fluent-style JSON REST API clients.

NuGet packages:
* [TypedRest](https://www.nuget.org/packages/TypedRest/)
* [TypedRest.CommandLine](https://www.nuget.org/packages/TypedRest.CommandLine/)


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
  public long Id { get; set; }
  public string Name { get; set; }
}
```


## Getting started

Install the `TypedRest` NuGet package in your REST client project.

You can then use the classes `EntryEndpoint`, `CollectionEndpoint`, `ElementEndpoint`, `TriggerEndpoint`, `PaginationEndpoint` and `StreamEndpoint` to build a local representation of a remote REST service. Based on our usecase sample this could look like this:
```cs
class SampleEntryEndpoint : EntryEndpoint
{
  public CollectionEndpoint<Package> Packages { get; }

  public SampleEntryEndpoint(Uri uri) : base(uri)
  {
    Packages = new CollectionEndpoint<Package>(this, relativeUri: "packages");
  }
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

Install the `TypedRest.CommmandLine` NuGet package in your command-line project. You can then use the classes `EntryCommand`, `CollectionCommand`, `ElementCommand`, `TriggerCommand`, `PaginationCommand` and `StreamCommand` to build command objects that parse arguments and operate on `EntryEndpoint`s, `CollectionEndpoint`s, `ElementEndpoint`s, `TriggerEndpoint`s, `PaginationEndpoint`s and `StreamEndpoint`s.
