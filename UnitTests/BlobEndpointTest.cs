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
                .Respond(new ByteArrayContent(data)
                {
                    Headers = {ContentType = new MediaTypeHeaderValue("mock/type")}
                });

            var stream = new MemoryStream();
            string mimeType = await _endpoint.DownloadToAsync(stream);

            stream.ToArray().Should().Equal(data);
            mimeType.Should().Be("mock/type");
        }

        [Test]
        public async Task TestUpload()
        {
            byte[] data = {1, 2, 3};

            Mock.Expect(HttpMethod.Put, "http://localhost/endpoint")
                .With(new ByteContentMatcher(data))
                .Respond(HttpStatusCode.NoContent);

            var stream = new MemoryStream(data);
            await _endpoint.UploadFromAsync(stream);
        }

        private class ByteContentMatcher : IMockedRequestMatcher
        {
            private readonly byte[] _data;

            public ByteContentMatcher(byte[] data)
            {
                _data = data;
            }

            public bool Matches(HttpRequestMessage message)
            {
                return message.Content.ReadAsByteArrayAsync().Result.SequenceEqual(_data);
            }
        }
    }
}