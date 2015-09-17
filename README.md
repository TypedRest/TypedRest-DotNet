# TypedRest

TypedRest helps you build type-safe fluent-style REST API clients.

NuGet package:
* [TypedRest](https://www.nuget.org/packages/TypedRest/)


## Usecase sample

We'll use this simple POCO (Plain old CLR object) class modelling software packages as an example:
```cs
class Package
{
  public long Id { get; set; }
  public string Name { get; set; }
}
```


## Getting started

Install the `TypedRest` NuGet package in your REST client project. You can then use the classes `RestEntryPoint`, `RestElementSet` and `RestElement` to build a local representation of a remote REST service. Based on our usecase sample this could look like this:
```cs
class SampleRestEntryPoint : RestEntryPoint
{
  public RestElementSet<Package> Packages { get; }

  public SampleRestEntryPoint([Uri uri) : base(uri)
  {
    Packages = new RestElementSet<Package>(this, relativeUri: "packages");
  }
}
```

You can then perform CRUD operations like this:
```cs
var server = new SampleRestEntryPoint(new Uri("http://myservice/api/"));
var packages = server.Packages.ReadAllAsync();
var resource = await server.Packages.CreateAsync(new Package {...});
var package = await server.Packages[1].ReadAsync();
await server.Packages[1].UpdateAsync(package);
await server.Packages[1].DeleteAsync();
```