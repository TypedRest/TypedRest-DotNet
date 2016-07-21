using Microsoft.Owin;
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
            app.UseWebApi(WebApiConfig.Build(UnityConfig.InitContainer()));
        }
    }
}
