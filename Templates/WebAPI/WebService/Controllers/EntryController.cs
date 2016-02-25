using System.Web.Http;
using WebApi.LinkHeader;

namespace XProjectNamespaceX.WebService.Controllers
{
    public class EntryController : ApiController
    {
        /// <summary>
        /// Displays the name of the API and provides HTTP Link headers for further navigation.
        /// </summary>
        [HttpGet, HttpHead, Route("")]
        [LinkHeader("entities/", Rel = "entities")]
        public string Read()
        {
            return "XProjectNameX API";
        }
    }
}