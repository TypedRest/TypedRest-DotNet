using System.Web.Http;

namespace TypedRest.Samples.Server
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            config.MapHttpAttributeRoutes();

            config.EnsureInitialized();
        }
    }
}