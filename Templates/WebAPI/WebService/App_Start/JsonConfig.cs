using System.Web.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace XProjectNamespaceX.WebService
{
    public static class JsonConfig
    {
        public static void ConfigureJson(this HttpConfiguration config)
        {
            var jsonSettings = config.Formatters.JsonFormatter.SerializerSettings;
            jsonSettings.DefaultValueHandling = DefaultValueHandling.Ignore;
            jsonSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            jsonSettings.Converters.Add(new StringEnumConverter { CamelCaseText = true });

            config.Formatters.Remove(config.Formatters.XmlFormatter);
        }
    }
}
