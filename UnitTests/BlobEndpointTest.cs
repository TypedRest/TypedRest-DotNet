using System.IO;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;

namespace TypedRest
{
    [TestFixture, Ignore("Server mock not implemented yet")]
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
            //stubFor(options(urlEqualTo("/endpoint"))
            //    .willReturn(aResponse()
            //        .withStatus(SC_OK)
            //        .withHeader("Allow", "PUT")));

            await _endpoint.ProbeAsync();

            _endpoint.DownloadAllowed.Should().BeFalse();
            _endpoint.UploadAllowed.Should().BeTrue();
        }

        [Test]
        public async Task TestDownload()
        {
            byte[] data = {1, 2, 3};

            //stubFor(get(urlEqualTo("/endpoint"))
            //        .willReturn(aResponse()
            //                .withStatus(SC_OK)
            //                .withBody(data)));

            var stream = new MemoryStream();
            await _endpoint.DownloadToAsync(stream);

            stream.ToArray().Should().Equal(data);
        }

        [Test]
        public async Task TestUpload()
        {
            byte[] data = {1, 2, 3};

            //stubFor(put(urlEqualTo("/endpoint"))
            //        .willReturn(aResponse()
            //                .withStatus(SC_NO_CONTENT)));

            var stream = new MemoryStream(data);
            await _endpoint.UploadFromAsync(stream);
        }
    }
}