using System.Collections.Generic;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Routing;
using Microsoft.Practices.Unity;
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
        /// Builds a Web API configuration object.
        /// </summary>
        /// <param name="container">The dependency injection container used to instantiate controllers.</param>
        public static HttpConfiguration Build(IUnityContainer container)
        {
            var config = new HttpConfiguration
            {
                DependencyResolver = new UnityDependencyResolver(container),
                Filters = {new ExceptionHandlingAttribute()},
                IncludeErrorDetailPolicy = IncludeErrorDetailPolicy.Always
            };
            config.MapHttpAttributeRoutes(new InheritanceRouteProvider());

            config.ConfigureJson();
            config.ConfigureSwagger();

            config.EnableVersionHeader();
            config.EnableBasicAuth();
            config.EnableRequestLogging();
            config.EnableHttpOptions();
            config.EnableHttpHead();

            config.EnsureInitialized();

            return config;
        }

        /// <summary>
        /// A custom WebAPI route provider that enables inheritance on controller attributes.
        /// </summary>
        private class InheritanceRouteProvider : DefaultDirectRouteProvider
        {
            protected override IReadOnlyList<IDirectRouteFactory> GetActionRouteFactories(HttpActionDescriptor actionDescriptor) =>
                actionDescriptor.GetCustomAttributes<IDirectRouteFactory>(inherit: true);
        }
    }
}
