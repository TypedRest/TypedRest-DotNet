---
uid: TypedRest.Endpoints.Rpc
summary: RPC endpoints allow you to interact with non-RESTful resources that act like callable functions.
---
Not every resource is a noun. For the ones that are verbs like "reindex", "calculate", or "send invitation", these endpoints model an HTTP POST as a method call. Which of the four types you pick follows the same reasoning as choosing between `Action`, `Action<T>`, `Func<T>` and `Func<TIn, TOut>` in C#: it depends on whether the call takes an entity, returns one, both or neither. See [Action](https://typedrest.net/endpoints/rpc/action/), [Consumer](https://typedrest.net/endpoints/rpc/consumer/), [Producer](https://typedrest.net/endpoints/rpc/producer/) and [Function](https://typedrest.net/endpoints/rpc/function/) endpoints.

Whether such an operation is available often depends on the current state of the system or the user's permissions. `ProbeAsync()` asks the server (via HTTP OPTIONS) and reflects the answer in `InvokeAllowed`, so a UI can grey out a button without duplicating the server's business rules.
