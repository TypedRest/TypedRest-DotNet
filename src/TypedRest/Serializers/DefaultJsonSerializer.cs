using System.Net.Http.Formatting;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace TypedRest.Serializers
{
    /// <summary>
    /// Handles serializing/deserializing from/to JSON using camel-case naming.
    /// </summary>
    public class DefaultJsonSerializer : JsonMediaTypeFormatter
    {
        public DefaultJsonSerializer()
        {
            SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            SerializerSettings.Converters.Add(new StringEnumConverter(new CamelCaseNamingStrategy()));
            SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
            SerializerSettings.TypeNameHandling = TypeNameHandling.Auto;
        }
    }
}
