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

`OAuthOptions` carries the settings for that flow. The `Action<OAuthOptions>` overload registers them as named options under the HTTP client's name, so different clients in the same application can authenticate against different identity servers and the settings can also come from configuration. To keep the client secret out of your code, build the options from configuration instead:

```csharp
services.AddTypedRest<MyClient>(new Uri("https://example.com/api/"))
        .AddOAuthHandler(_ => configuration.GetSection("OAuth").Get<OAuthOptions>()!);
```

`OAuthHandler` sits in the `HttpClient` pipeline and keeps authentication out of your calling code entirely. On the first request it looks up the token endpoint via OpenID Connect discovery, performs a client credentials flow and attaches the resulting token as a `Bearer` header. The token is then reused for subsequent requests until shortly before it expires, so the identity server is contacted only occasionally rather than per request. If a server rejects a token as invalid anyway, the request is retried once with a freshly requested one.
