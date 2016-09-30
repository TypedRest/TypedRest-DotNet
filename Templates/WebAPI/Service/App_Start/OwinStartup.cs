using JetBrains.Annotations;
using Microsoft.Owin;
using Microsoft.Practices.Unity;
using NLog.Owin.Logging;
using Owin;

[assembly: OwinStartup(typeof(XProjectNamespaceX.Service.OwinStartup))]

namespace XProjectNamespaceX.Service
{
    /// <summary>
    /// Entry point for OWIN.
    /// </summary>
    public class OwinStartup
    {
        private readonly IUnityContainer _container;

        /// <summary>
        /// Stand-alone OWIN startup.
        /// </summary>
        [UsedImplicitly]
        public OwinStartup()
        {
            _container = UnityConfig.InitContainer();
        }

        /// <summary>
        /// OWIN startup using an already existing dependency injection <paramref name="container"/>.
        /// </summary>
        public OwinStartup(IUnityContainer container)
        {
            _container = container;
        }

        public void Configuration(IAppBuilder app)
        {
            app
                .UseNLog()
                .UseWebApi(WebApiConfig.Build(_container));
        }
    }
}
