---
uid: TypedRest.CommandLine.Commands.Generic
summary: Commands for operating on <xref:TypedRest.Endpoints.Generic>.
---
These commands turn the methods of the collection and element endpoints into verbs. A collection command handles `create`, `create-all` and `set-all`, lists all entities when given no arguments and reads a page when given a range like `0-9`. Any other argument is not a verb but an element ID: it is looked up in the endpoint and the remaining arguments are handed to the element's own command, which in turn understands `set`, `merge` and `delete` and prints the entity when given nothing.

```sh
myapp contacts                       # ReadAllAsync()
myapp contacts 0-9                   # ReadRangeAsync(...)
myapp contacts create '{"name":"..."}' # CreateAsync(...)
myapp contacts 1337                  # ReadAsync()
myapp contacts 1337 delete           # DeleteAsync()
```

Entities are taken from the next argument as JSON, or read from stdin if it is missing, and printed as JSON. Override `InputEntity()` and `OutputEntity()` to accept a friendlier argument syntax or to render a table instead.

To expose resources nested below an element, override `GetElementCommand()` and return your own command type for it; this is the command-side counterpart to deriving from `ElementEndpoint` to add child endpoints.

Use an indexer command when elements are addressable by ID but the collection as a whole is not listable; it only does the ID lookup and dispatch, without any collection verbs.
