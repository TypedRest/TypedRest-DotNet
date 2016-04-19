using System.Web.Http;
using NLog;
using Unity.WebApi;
using WebApi.BasicAuth;
using WebApi.HttpOptions;

namespace XProjectNamespaceX.WebService
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            config.MapHttpAttributeRoutes();

            config.MessageHandlers.Add(new LoggingHandler());

            config.DependencyResolver = new UnityDependencyResolver(UnityConfig.InitContainer());

            config.ConfigureJson();
            config.EnableBasicAuth();
            config.EnableHttpOptions();
            config.EnableHttpHead();
            config.EnableSwagger();
        }
    }
}
