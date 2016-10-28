using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Web.Http;
using System.Web.Http.Description;
using WebApi.LinkHeader;

namespace XProjectNamespaceX.WebService.Controllers
{
    /// <summary>
    /// Entry point for XProjectNameX API.
    /// </summary>
    [ApiExplorerSettings(IgnoreApi = true)]
    public class EntryController : ApiController
    {
        /// <summary>
        /// Displays the name of the API and provides HTTP Link headers for further navigation.
        /// </summary>
        [HttpGet, Route("")]
        [LinkHeader("entities/", Rel = "entities")]
        public IHttpActionResult Read() => Ok("XProjectNameX API")
            .WithLink("swagger", rel: "swagger")
            .WithLink("cli-client", rel: "cli-client")
            .WithLink("entities/", rel: "entities");

        /// <summary>
        /// Provides a download of a command-line client executable for the API.
        /// </summary>
        [HttpGet, Route("cli-client")]
        public HttpResponseMessage DownloadCliClient() => new HttpResponseMessage
        {
            Content = new StreamContent(Assembly.GetAssembly(typeof(EntryController))
                .GetManifestResourceStream(typeof(EntryController), "XProjectNamespaceX-Client.exe"))
            {
                Headers =
                {
                    ContentDisposition = new ContentDispositionHeaderValue("attachment") {FileName = "XProjectNamespaceX-Client.exe"}
                }
            }
        };
    }
}