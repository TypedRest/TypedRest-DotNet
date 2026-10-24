namespace TypedRest.OAuth;

public class OAuthHandlerTest : IDisposable
{
    private readonly MockHttpMessageHandler _mock = new(BackendDefinitionBehavior.Always);
    private readonly HttpClient _client;

    public OAuthHandlerTest()
    {
        _mock.When(HttpMethod.Get, "http://localhost/auth/.well-known/openid-configuration")
             .Respond("application/json", """{"issuer": "http://localhost/auth", "token_endpoint": "http://localhost/auth/token", "jwks_uri": "http://localhost/auth/jwks"}""");
        _mock.When(HttpMethod.Get, "http://localhost/auth/jwks")
             .Respond("application/json", """{"keys": []}""");
        _mock.When(HttpMethod.Post, "http://localhost/auth/token")
             .Respond("application/json", """{"access_token": "token", "token_type": "Bearer", "expires_in": 3600}""");

        _client = new(new OAuthHandler(new() {Uri = new("http://localhost/auth"), ClientId = "client", ClientSecret = "secret"}, _mock));
    }

    public void Dispose()
    {
        _client.Dispose();
        _mock.VerifyNoOutstandingExpectation();
    }

    private static HttpResponseMessage InvalidToken(HttpRequestMessage request)
        => new(HttpStatusCode.Unauthorized) {Headers = {WwwAuthenticate = {new("Bearer", "error=\"invalid_token\"")}}};

    [Fact]
    public async Task TestRetryString()
    {
        _mock.Expect(HttpMethod.Post, "http://localhost/endpoint")
             .Respond(InvalidToken);
        _mock.Expect(HttpMethod.Post, "http://localhost/endpoint")
             .WithContent("data")
             .Respond(HttpStatusCode.NoContent);

        var response = await _client.PostAsync("http://localhost/endpoint", new StringContent("data"), TestContext.Current.CancellationToken);
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task TestNoRetryStream()
    {
        _mock.Expect(HttpMethod.Post, "http://localhost/endpoint")
             .Respond(InvalidToken);

        byte[] data = [1, 2, 3];
        using var stream = new MemoryStream(data);
        var response = await _client.PostAsync("http://localhost/endpoint", new StreamContent(stream), TestContext.Current.CancellationToken);
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task TestNoRetryMultipartStream()
    {
        _mock.Expect(HttpMethod.Post, "http://localhost/endpoint")
             .Respond(InvalidToken);

        byte[] data = [1, 2, 3];
        using var stream = new MemoryStream(data);
        var content = new MultipartFormDataContent {{new StreamContent(stream), "data", "file.dat"}};
        var response = await _client.PostAsync("http://localhost/endpoint", content, TestContext.Current.CancellationToken);
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}
