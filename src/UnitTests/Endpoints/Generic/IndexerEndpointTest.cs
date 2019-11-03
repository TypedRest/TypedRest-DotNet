using System;
using FluentAssertions;
using Xunit;

namespace TypedRest.Endpoints.Generic
{
    [Collection("Endpoint")]
    public class IndexerEndpointTest : EndpointTestBase
    {
        [Fact]
        public void TestGetById()
        {
            var endpoint = new IndexerEndpoint<ElementEndpoint<MockEntity>>(EntryEndpoint, "endpoint");

            endpoint["1"].Uri.Should().Be(new Uri("http://localhost/endpoint/1"));
        }

        [Fact]
        public void RejectsTypesWithoutSuitablePublicConstructor()
        {
            var endpoint = new IndexerEndpoint<EndpointBase>(EntryEndpoint, "endpoint");
            Func<EndpointBase> getter = () => endpoint["1"];
            getter.Should().Throw<TypeInitializationException>();
        }
    }
}
