using System.Web.Http;

namespace TypedRest.Samples.Server.Controllers
{
    public class EntryController
    {
        /// <summary>
        /// Displays the name of the API and provides HTTP Link headers for further navigation.
        /// </summary>
        [HttpGet, HttpHead, Route("")]
        public string Read()
        {
            return "Sample API";
        }
    }
}