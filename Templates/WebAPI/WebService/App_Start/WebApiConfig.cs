using System.Collections.Generic;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Routing;
using Unity.WebApi;
using WebApi.BasicAuth;
using WebApi.HttpOptions;
using WebApi.RequestLogging;

namespace XProjectNamespaceX.WebService
{
    /// <summary>
    /// Configuration for Web API.
    /// </summary>
    public static class WebApiConfig
    {
        /// <summary>
        /// Configures Web API and related services. Instantiates a dependency injection container.
        /// </summary>
        public static void Register(HttpConfiguration config)
        {
            config.DependencyResolver = new UnityDependencyResolver(UnityConfig.InitContainer());
            config.MapHttpAttributeRoutes(new InheritanceRouteProvider());

            config.ConfigureJson();
            config.ConfigureSwagger();
            config.Filters.Add(new ExceptionHandlingAttribute());
            config.IncludeErrorDetailPolicy = IncludeErrorDetailPolicy.Always;

            config.EnableVersionHeader();
            config.EnableBasicAuth();
            config.EnableRequestLogging();
            config.EnableHttpOptions();
            config.EnableHttpHead();

            config.EnsureInitialized();
        }

        /// <summary>
        /// A custom WebAPI route provider that enables inheritance on controller attributes.
        /// </summary>
        private class InheritanceRouteProvider : DefaultDirectRouteProvider
        {
            protected override IReadOnlyList<IDirectRouteFactory> GetActionRouteFactories(
                HttpActionDescriptor actionDescriptor)
            {
                return actionDescriptor.GetCustomAttributes<IDirectRouteFactory>(inherit: true);
            }
        }
    }
}
