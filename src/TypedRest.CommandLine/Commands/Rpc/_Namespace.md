---
uid: TypedRest.CommandLine.Commands.Rpc
summary: Commands for operating on <xref:TypedRest.Endpoints.Rpc>.
---
One command per RPC endpoint type. Since such an endpoint offers exactly one operation, these commands take no verb: reaching the command is the invocation.

```sh
myapp reindex                       # ActionEndpoint.InvokeAsync()
myapp quote '{"weight":2.5}'        # FunctionEndpoint.InvokeAsync(...)
```

Whether a command expects an entity and whether it prints one follows from the endpoint type it wraps. Input is taken from the next argument as JSON, or read from stdin if that argument is missing; results are printed as JSON. Override `InputEntity()` to build the entity from friendlier arguments, or `OutputEntity()` to format the result differently.
