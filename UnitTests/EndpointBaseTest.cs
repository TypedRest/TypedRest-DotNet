using System;
using FluentAssertions;
using NUnit.Framework;

namespace TypedRest
{
    [TestFixture]
    public class EndpointBaseTest : EndpointTestBase
    {
        private ResourceCollectionEndpoint _endpoint;

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();
            _endpoint = new ResourceCollectionEndpoint(EntryEndpoint,"resources");
        }

        [Test]
        public void TestUriPathFromSubresourceContainsIdentitfier()
        {
            _endpoint["4711"].Subresource.Uri.ToString().Should().Be("http://localhost/resources/4711/subresource");
        }

        private class ResourceCollectionEndpoint : CollectionEndpointBase<MockEntity, ResourceEndpoint>
        {
            public ResourceCollectionEndpoint(IEndpoint parent, string relativeUri) : base(parent, relativeUri)
            {
            }

            #region Overrides of CollectionEndpointBase<MockEntity,ResourceEndpoint>

            public override ResourceEndpoint this[Uri relativeUri] => new ResourceEndpoint(this, relativeUri);

            #endregion
        }

        private class ResourceEndpoint : ElementEndpoint<MockEntity>
        {
            public ResourceEndpoint(IEndpoint parent, Uri relativeUri) 
                : base(parent, relativeUri)
            {
            }

            public SubresourceEndpoint Subresource => new SubresourceEndpoint(this, "subresource");
        }

        private class SubresourceEndpoint : ElementEndpoint<MockEntity>
        {
            public SubresourceEndpoint(IEndpoint parent, string relativeUri) 
                : base(parent, relativeUri)
            {
            }
        }
    }
}