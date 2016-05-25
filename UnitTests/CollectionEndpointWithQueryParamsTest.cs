using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using RichardSzalay.MockHttp;

namespace TypedRest
{
    [TestFixture]
    public class CollectionEndpointWithQueryParamsTest : EndpointTestBase
    {
        private SearchableMockEntityCollectionEndpoint _endpoint;

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();
            _endpoint = new SearchableMockEntityCollectionEndpoint(EntryEndpoint, "endpoint");
        }

        [Test(Description = "Proves fix for the bug described in https://github.com/1and1/TypedRest-DotNet/issues/9")]
        public async Task TestGetByEntityWithLinkHeaderAndQueryParamsAbsolute()
        {
            Mock.Expect(HttpMethod.Get, "http://localhost/endpoint/")
                .Respond(new HttpResponseMessage
                {
                    Content = new StringContent("[]", Encoding.UTF8, JsonMime),
                    Headers = { { "Link", "<http://localhost/endpoint/?{email,ref}>; rel=search; templated=true" } }
                });

            await _endpoint.Search(name: "john").ReadAllAsync();

            _endpoint.Search(name: "john").Uri.ToString()
                .Should().Be(new Uri("http://localhost/endpoint/?name=john/").ToString());
        }

        public class SearchableMockEntityCollectionEndpoint : CollectionEndpoint<MockEntity>
        {
            public SearchableMockEntityCollectionEndpoint(IEndpoint parent, string relativeUri, bool ensureTrailingSlashOnParentUri = false)
                : base(parent, relativeUri, ensureTrailingSlashOnParentUri)
            {
                AddDefaultLinkTemplate("{?name}", "search");
            }

            public CollectionEndpoint<MockEntity> Search(string name = null)
            {
                return new CollectionEndpoint<MockEntity>(this, LinkTemplate("search").Resolve(new { name }));
            }
        }
    }
}