using TypedRest.Serializers;

namespace TypedRest.Endpoints.Generic;

[Collection("Endpoint")]
public class NewtonsoftJsonElementEndpointTest() : ElementEndpointTestBase(new NewtonsoftJsonSerializer())
{
    [Fact]
    public async Task TestJsonPatch()
    {
        Mock.Expect(HttpMethods.Patch, "http://localhost/endpoint")
            .WithContent("""[{"value":"testX","path":"/name","op":"replace"}]""")
            .Respond(JsonMime, """{"id":5,"name":"testX"}""");

        var result = await Endpoint.UpdateAsync(patch => patch.Replace(x => x.Name, "testX"));
        result.Should().Be(new MockEntity(5, "testX"));
    }

    [Fact]
    public async Task TestJsonPatchFallback()
    {
        Mock.Expect(HttpMethods.Patch, "http://localhost/endpoint")
            .Respond(HttpStatusCode.MethodNotAllowed);

        Mock.Expect(HttpMethod.Get, "http://localhost/endpoint")
            .Respond(_ => new()
             {
                 Content = new StringContent("""{"id":5,"name":"test1"}""", Encoding.UTF8, JsonMime)
             });
        Mock.Expect(HttpMethod.Put, "http://localhost/endpoint")
            .WithContent("""{"id":5,"name":"testX"}""")
            .Respond(JsonMime, """{"id":5,"name":"testX"}""");

        var result = await Endpoint.UpdateAsync(patch => patch.Replace(x => x.Name, "testX"));
        result.Should().Be(new MockEntity(5, "testX"));
    }
}
