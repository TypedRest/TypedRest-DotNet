namespace TypedRest.Endpoints.Raw;

[Collection("Endpoint")]
public class BlobEndpointTest : EndpointTestBase
{
    private readonly BlobEndpoint _endpoint;

    public BlobEndpointTest()
    {
        _endpoint = new(EntryEndpoint, "endpoint");
    }

    [Fact]
    public async Task TestProbe()
    {
        Mock.Expect(HttpMethod.Options, "http://localhost/endpoint")
            .Respond(_ => new StringContent("") {Headers = {Allow = {HttpMethod.Put.Method}}});

        await _endpoint.ProbeAsync();

        _endpoint.DownloadAllowed.Should().BeFalse();
        _endpoint.UploadAllowed.Should().BeTrue();
    }

    [Fact]
    public async Task TestDownload()
    {
        byte[] data = [1, 2, 3];

        Mock.Expect(HttpMethod.Get, "http://localhost/endpoint")
            .Respond(_ => new ByteArrayContent(data));

        using var downloadStream = await _endpoint.DownloadAsync();
        using var memStream = new MemoryStream();
        await downloadStream.CopyToAsync(memStream);
        memStream.ToArray().Should().Equal(data);
    }

    [Fact]
    public async Task TestUpload()
    {
        byte[] data = [1, 2, 3];

        Mock.Expect(HttpMethod.Put, "http://localhost/endpoint")
            .With(new ByteContentMatcher(data, mimeType: "mock/type"))
            .Respond(HttpStatusCode.NoContent);

        using var stream = new MemoryStream(data);
        await _endpoint.UploadFromAsync(stream, mimeType: "mock/type");
    }
}