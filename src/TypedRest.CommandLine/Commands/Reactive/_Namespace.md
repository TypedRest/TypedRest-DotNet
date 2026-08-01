---
uid: TypedRest.CommandLine.Commands.Reactive
summary: Commands for operating on <xref:TypedRest.Endpoints.Reactive>.
---
> [!NOTE]
> These commands require the [TypedRest.Reactive](https://www.nuget.org/packages/TypedRest.Reactive/) package.

Just as the reactive endpoints extend their pull-based counterparts, these commands extend the element and collection commands with one extra verb each. Everything else keeps working as before, so the same command can both fetch the current state and follow it live.

```sh
myapp jobs 1337        # read the job once
myapp jobs 1337 poll   # keep printing it as it changes
myapp messages stream  # print messages as they are appended
myapp messages stream 5
```

The trailing argument of `stream` is the index to start from; a negative value counts back from the end of the collection.

Output goes through a <xref:TypedRest.CommandLine.IO.StreamPrinter`1>, which subscribes to the endpoint's observable and prints entities as they arrive, running until the server ends the stream or the user aborts. Override `OutputEntitiesAsync()` to render the stream differently, e.g. as a continuously updated table.
