using TypedRest.Serializers;

namespace TypedRest.Endpoints.Generic;

[Collection("Endpoint")]
public class SystemTextJsonElementEndpointTest() : ElementEndpointTestBase(new SystemTextJsonSerializer());
