using System.Net.Http.Formatting;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace TypedRest
{
    public class DefaultJsonSerializer : JsonMediaTypeFormatter
    {
        public DefaultJsonSerializer()
        {
            SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            SerializerSettings.Converters.Add(new StringEnumConverter {CamelCaseText = true});
            SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
            SerializerSettings.TypeNameHandling = TypeNameHandling.Auto;
        }
    }
}
