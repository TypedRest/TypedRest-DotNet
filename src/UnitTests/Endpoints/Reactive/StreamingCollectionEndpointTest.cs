namespace TypedRest.Endpoints.Reactive
{
    [Collection("Endpoint")]
    public class StreamingCollectionEndpointTest : EndpointTestBase
    {
        private readonly StreamingCollectionEndpoint<MockEntity> _endpoint;

        public StreamingCollectionEndpointTest()
        {
            _endpoint = new(EntryEndpoint, "endpoint");
        }

        [Fact]
        public void TestGetObservable()
        {
            Mock.Expect(HttpMethod.Get, "http://localhost/endpoint")
                .WithHeaders("Range", "elements=0-")
                .Respond(HttpStatusCode.PartialContent,
                     new StringContent("[{\"id\":5,\"name\":\"test1\"},{\"id\":6,\"name\":\"test2\"}]", Encoding.UTF8, JsonMime)
                     {
                         Headers = {ContentRange = new(from: 0, to: 1) {Unit = "elements"}}
                     });

            Mock.Expect(HttpMethod.Get, "http://localhost/endpoint")
                .WithHeaders("Range", "elements=2-")
                .Respond(HttpStatusCode.PartialContent,
                     new StringContent("[{\"id\":7,\"name\":\"test3\"}]", Encoding.UTF8, JsonMime)
                     {
                         Headers = {ContentRange = new(from: 2, to: 2, length: 3) {Unit = "elements"}}
                     });

            var observable = _endpoint.GetObservable();
            observable.ToEnumerable().ToList().Should().Equal(
                new MockEntity(5, "test1"),
                new MockEntity(6, "test2"),
                new MockEntity(7, "test3"));
        }

        [Fact]
        public void TestGetObservableOffset()
        {
            Mock.Expect(HttpMethod.Get, "http://localhost/endpoint")
                .WithHeaders("Range", "elements=2-")
                .Respond(HttpStatusCode.PartialContent,
                     new StringContent("[{\"id\":7,\"name\":\"test3\"}]", Encoding.UTF8, JsonMime)
                     {
                         Headers = {ContentRange = new(from: 2, to: 2, length: 3) {Unit = "elements"}}
                     });

            var observable = _endpoint.GetObservable(startIndex: 2);
            observable.ToEnumerable().ToList().Should().Equal(new MockEntity(7, "test3"));
        }

        [Fact]
        public void TestGetObservableTail()
        {
            Mock.Expect(HttpMethod.Get, "http://localhost/endpoint")
                .WithHeaders("Range", "elements=-1")
                .Respond(HttpStatusCode.PartialContent,
                     new StringContent("[{\"id\":7,\"name\":\"test3\"}]", Encoding.UTF8, JsonMime)
                     {
                         Headers = {ContentRange = new(from: 2, to: 2, length: 3) {Unit = "elements"}}
                     });

            var observable = _endpoint.GetObservable(startIndex: -1);
            observable.ToEnumerable().ToList().Should().Equal(new MockEntity(7, "test3"));
        }
    }
}
