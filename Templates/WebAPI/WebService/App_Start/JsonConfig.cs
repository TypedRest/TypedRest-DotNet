using System.Web.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace XProjectNamespaceX.WebService
{
    /// <summary>
    /// Configuration of JSON serialization of model classes used by the Web API interface.
    /// </summary>
    public static class JsonConfig
    {
        private static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            Converters = {new StringEnumConverter {CamelCaseText = true}},
            NullValueHandling = NullValueHandling.Ignore
        };

        /// <summary>
        /// Configures the JSON serialization of model classes used by the Web API interface. Disables XML serialization.
        /// </summary>
        public static void ConfigureJson(this HttpConfiguration config)
        {
            config.Formatters.JsonFormatter.SerializerSettings = Settings;
            config.Formatters.Remove(config.Formatters.XmlFormatter);
        }

        /// <summary>
        /// Serializes an object to JSON using the same configuration applied to Web API models.
        /// </summary>
        public static string ToJson<T>(this T obj) => JsonConvert.SerializeObject(obj, Settings);

        /// <summary>
        /// Deserializes an object from JSON using the same configuration applied to Web API models.
        /// </summary>
        public static T ParseJson<T>(this string json) => JsonConvert.DeserializeObject<T>(json, Settings);
    }
}
