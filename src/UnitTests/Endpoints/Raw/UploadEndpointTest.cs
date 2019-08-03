using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using RichardSzalay.MockHttp;
using Xunit;

namespace TypedRest.Endpoints.Raw
{
    [Collection("Endpoint")]
    public class UploadEndpointTest : EndpointTestBase
    {
        [Fact]
        public async Task TestUploadRaw()
        {
            var endpoint = new UploadEndpoint(EntryEndpoint, "endpoint");

            byte[] data = {1, 2, 3};

            Mock.Expect(HttpMethod.Post, "http://localhost/endpoint")
                .With(new ByteContentMatcher(data, mimeType: "mock/type"))
                .Respond(HttpStatusCode.NoContent);

            using (var stream = new MemoryStream(data))
                await endpoint.UploadFromAsync(stream, mimeType: "mock/type");
        }

        [Fact]
        public async Task TestUploadForm()
        {
            var endpoint = new UploadEndpoint(EntryEndpoint, "endpoint", formField: "data");

            byte[] data = {1, 2, 3};

            Mock.Expect(HttpMethod.Post, "http://localhost/endpoint")
                .With(new MultipartFormContentMatcher("data", data, mimeType: "mock/type", fileName: "file.dat"))
                .Respond(HttpStatusCode.NoContent);

            using (var stream = new MemoryStream(data))
                await endpoint.UploadFromAsync(stream, mimeType: "mock/type", fileName: "file.dat");
        }
    }
}
