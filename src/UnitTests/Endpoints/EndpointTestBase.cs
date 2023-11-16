namespace TypedRest.Endpoints;

public abstract class EndpointTestBase : IDisposable
{
    public const string JsonMime = "application/json";

    protected readonly MockHttpMessageHandler Mock = new();
    protected readonly IEndpoint EntryEndpoint;

    protected EndpointTestBase(MediaTypeFormatter? serializer = null)
    {
        EntryEndpoint = new MockEntryEndpoint(Mock, serializer);
    }

    public void Dispose() => Mock.VerifyNoOutstandingExpectation();

    private class MockEntryEndpoint(HttpMessageHandler messageHandler, MediaTypeFormatter? serializer)
        : EntryEndpoint(new HttpClient(messageHandler), new Uri("http://localhost/"), serializer);
}
