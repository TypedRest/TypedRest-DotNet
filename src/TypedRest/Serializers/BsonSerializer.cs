using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace TypedRest.Serializers;

/// <summary>
/// Handles serializing/deserializing from/to BSON using <see cref="Newtonsoft.Json"/>.
/// Uses camel-case naming and does not serialize <c>null</c> by default.
/// </summary>
public class BsonSerializer : BsonMediaTypeFormatter
{
    public BsonSerializer()
    {
        SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
        SerializerSettings.Converters.Add(new StringEnumConverter(new CamelCaseNamingStrategy()));
        SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
        SerializerSettings.TypeNameHandling = TypeNameHandling.Auto;
    }
}
