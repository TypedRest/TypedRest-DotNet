using TypedRest.Serializers;

namespace TypedRest.Endpoints.Generic;

[Collection("Endpoint")]
public class SystemTextJsonElementEndpointTest : ElementEndpointTestBase
{
    public SystemTextJsonElementEndpointTest()
        : base(new SystemTextJsonSerializer())
    {}
}
