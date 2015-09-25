using System;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;

namespace TypedRest
{
    [TestFixture, Ignore("Server mock not implemented yet")]
    public class RestSetTests : RestEndpointTest
    {
        private IRestSet<MockEntity, RestElement<MockEntity>> _endpoint;

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();
            _endpoint = new RestSet<MockEntity>(EntryPoint, "endpoint");
        }

        [Test]
        public async Task TestReadAll()
        {
            //stubFor(get(urlEqualTo("/endpoint/"))
            //    .withHeader("Accept", equalTo(jsonMime))
            //    .willReturn(aResponse()
            //        .withStatus(200)
            //        .withHeader("Content-Type", jsonMime)
            //        .withBody("[{\"id\":5,\"name\":\"test1\"},{\"id\":6,\"name\":\"test2\"}]")));

            var result = await _endpoint.ReadAllAsync();
            result.Should().Equal(new MockEntity {Id = 5, Name = "test"});
        }

        [Test]
        public async Task TestCreate()
        {
            var location = new Uri("/endpoint/new", UriKind.Relative);
            //stubFor(post(
            //    urlEqualTo("/endpoint/"))
            //    .withRequestBody(equalToJson("{\"id\":5,\"name\":\"test\"}"))
            //    .willReturn(aResponse()
            //        .withStatus(201)
            //        .withHeader("Location", location.toASCIIString())));

            var element = await _endpoint.CreateAsync(new MockEntity {Id = 5, Name = "test"});
            element.Uri.Should().Be(location);
        }
    }
}