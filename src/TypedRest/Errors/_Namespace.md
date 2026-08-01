---
uid: TypedRest.Errors
summary: '[Handling errors](https://typedrest.net/error-handling/) in HTTP responses.'
---
Every response an endpoint receives is passed through an `IErrorHandler` before the endpoint looks at it. This keeps status code checks out of your calling code: an error simply surfaces as an exception.

The default handler maps status codes to the .NET exception types you would use for the equivalent local operation, letting callers catch familiar types instead of inspecting `HttpRequestException`. See [Exception mapping](https://typedrest.net/error-handling/) for the full table.

It takes the message from the `message` or `details` field of a JSON error body when present, keeps the original `HttpRequestException` as the inner exception, and attaches the response headers and body to the exception's `Data` dictionary, where `GetHttpResponseHeaders()` and `GetHttpResponseBody()` can retrieve them.

To adapt this to an API with its own error format, derive from `DefaultErrorHandler` and override `ExtractMessage()` or `MapException()`, then pass an instance to the `EntryEndpoint` constructor. All child endpoints inherit it.

```csharp
class MyClient(Uri uri) : EntryEndpoint(uri, errorHandler: new MyErrorHandler())
{
    // ...
}
```
