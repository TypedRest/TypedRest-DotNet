using NUnit.Framework;
using XProjectNamespaceX.Model;
using TypedRest;

namespace XProjectNamespaceX.Client
{
    [TestFixture]
    public class EntitiesEndpointTest : EndpointTestBase
    {
        private CollectionEndpoint<MyEntity> _endpoint;

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();
            _endpoint = new CollectionEndpoint<MyEntity>(EntryEndpoint, "endpoint");
        }
    }
}
