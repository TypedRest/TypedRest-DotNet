# ![TypedRest](https://raw.githubusercontent.com/TypedRest/TypedRest-DotNet/master/logo.svg) System.Text.Json

Adds support for serializing with [System.Text.Json](https://learn.microsoft.com/en-us/dotnet/api/system.text.json) to [TypedRest](https://www.nuget.org/packages/TypedRest/), instead of the default [Newtonsoft.Json](https://www.newtonsoft.com/json).

    dotnet add package TypedRest.SystemTextJson

Pass a `SystemTextJsonSerializer` to the `EntryEndpoint` constructor:

```csharp
class MyClient(Uri uri) : EntryEndpoint(uri, serializer: new SystemTextJsonSerializer())
{
    // ...
}
```

It uses web defaults (camel-case property naming) and omits `null` values when writing. Customize it via `Options`:

```csharp
var endpoint = new EntryEndpoint(
    new Uri("http://example.com/"),
    serializer: new SystemTextJsonSerializer
    {
        Options =
        {
            WriteIndented = true,
            Converters = {new JsonStringEnumConverter()}
        }
    });
```

Note that `ElementEndpoint.UpdateAsync(Action<JsonPatchDocument<T>>)` requires a Newtonsoft.Json-based serializer and throws `NotSupportedException` when only this one is configured. The other update overloads are unaffected.

## Links

- [Serializers documentation](https://typedrest.net/serializers/json/)
- [API documentation](https://dotnet.typedrest.net/)
