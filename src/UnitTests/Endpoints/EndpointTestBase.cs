namespace TypedRest.Endpoints;

public abstract class EndpointTestBase : IDisposable
{
    public const string JsonMime = "application/json";

    protected readonly MockHttpMessageHandler Mock = new();
    protected readonly IEndpoint EntryEndpoint;

    protected EndpointTestBase()
    {
        EntryEndpoint = new MockEntryEndpoint(Mock);
    }

    public void Dispose() => Mock.VerifyNoOutstandingExpectation();

    private class MockEntryEndpoint : EntryEndpoint
    {
        public MockEntryEndpoint(HttpMessageHandler messageHandler)
            : base(new HttpClient(messageHandler), new Uri("http://localhost/"))
        {}
    }
}