# ![TypedRest](https://raw.githubusercontent.com/TypedRest/TypedRest-DotNet/master/logo.svg) OAuth

Adds support for [OAuth 2.0](https://oauth.net/2/) / [OpenID Connect](https://openid.net/connect/) authentication to [HttpClient](https://learn.microsoft.com/en-us/dotnet/api/system.net.http.httpclient).

    dotnet add package TypedRest.OAuth

This provides an [HttpClient DelegatingHandler](https://learn.microsoft.com/en-us/dotnet/api/system.net.http.delegatinghandler) that transparently requests access tokens with a client secret and caches them until they expire. It can be used independently of the other [TypedRest](https://www.nuget.org/packages/TypedRest/) packages.

Call `.AddOAuthHandler()` after `.AddTypedRest()`:

```csharp
services.AddTypedRest<MyClient>(new Uri("https://example.com/api/"))
        .AddOAuthHandler(new Uri("https://identity.example.com/"), clientId: "my-client", clientSecret: "…");
```

or after `.AddHttpClient()` when you are not using the main TypedRest package:

```csharp
services.AddHttpClient<MyService>()
        .AddOAuthHandler(options =>
        {
            options.Uri = new Uri("https://identity.example.com/");
            options.ClientId = "my-client";
            options.ClientSecret = "…";
        });
```

The `Action<OAuthOptions>` overload binds through the options system under the HTTP client's name, so the settings can also come from configuration.
