using Microsoft.Owin;
using NLog.Owin.Logging;
using Owin;

[assembly: OwinStartup(typeof(XProjectNamespaceX.WebService.OwinStartup))]

namespace XProjectNamespaceX.WebService
{
    /// <summary>
    /// Entry point for OWIN.
    /// </summary>
    public class OwinStartup
    {
        public void Configuration(IAppBuilder app)
        {
            app
                .UseNLog()
                .UseWebApi(WebApiConfig.Build(UnityConfig.InitContainer()));
        }
    }
}
