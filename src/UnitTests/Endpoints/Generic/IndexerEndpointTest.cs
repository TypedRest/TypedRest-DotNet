namespace TypedRest.Endpoints.Generic;

[Collection("Endpoint")]
public class IndexerEndpointTest : EndpointTestBase
{
    private readonly IndexerEndpoint<ElementEndpoint<MockEntity>> _endpoint;

    public IndexerEndpointTest()
    {
        _endpoint = new(EntryEndpoint, "endpoint");
    }

    [Fact]
    public void TestGetById()
    {
        _endpoint["x/y"].Uri.Should().Be(new Uri("http://localhost/endpoint/x%2Fy"));
    }

    [Fact]
    public void RejectsTypesWithoutSuitablePublicConstructor()
    {
        Func<EndpointBase> getter = () => new IndexerEndpoint<EndpointBase>(EntryEndpoint, "endpoint")["1"];
        getter.Should().Throw<TypeInitializationException>();
    }
}