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
        public string Read()
        {
            return "XProjectNameX API";
        }
    }
}