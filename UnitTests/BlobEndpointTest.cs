using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using RichardSzalay.MockHttp;

namespace TypedRest
{
    [TestFixture]
    public class BlobEndpointTest : EndpointTestBase
    {
        private BlobEndpoint _endpoint;

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();
            _endpoint = new BlobEndpoint(EntryEndpoint, "endpoint");
        }

        [Test]
        public async Task TestProbe()
        {
            Mock.Expect(HttpMethod.Options, "http://localhost/endpoint")
                .Respond(new StringContent("") {Headers = {Allow = {HttpMethod.Put.Method}}});

            await _endpoint.ProbeAsync();

            _endpoint.DownloadAllowed.Should().BeFalse();
            _endpoint.UploadAllowed.Should().BeTrue();
        }

        [Test]
        public async Task TestDownload()
        {
            byte[] data = {1, 2, 3};

            Mock.Expect(HttpMethod.Get, "http://localhost/endpoint")
                .Respond(new ByteArrayContent(data));

            using (var downloadStream = await _endpoint.DownloadAsync())
            using (var memStream = new MemoryStream())
            {
                await downloadStream.CopyToAsync(memStream);
                memStream.ToArray().Should().Equal(data);
            }
        }

        [Test]
        public async Task TestUpload()
        {
            byte[] data = {1, 2, 3};

            Mock.Expect(HttpMethod.Put, "http://localhost/endpoint")
                .With(new ByteContentMatcher(data, mimeType: "mock/type"))
                .Respond(HttpStatusCode.NoContent);

            using (var stream = new MemoryStream(data))
                await _endpoint.UploadFromAsync(stream, mimeType: "mock/type");
        }

        private class ByteContentMatcher : IMockedRequestMatcher
        {
            private readonly byte[] _data;
            private readonly string _mimeType;

            public ByteContentMatcher(byte[] data, string mimeType)
            {
                _data = data;
                _mimeType = mimeType;
            }

            public bool Matches(HttpRequestMessage message)
            {
                return
                    message.Content.ReadAsByteArrayAsync().Result.SequenceEqual(_data) &&
                    message.Content.Headers.ContentType.MediaType == _mimeType;
            }
        }
    }
}