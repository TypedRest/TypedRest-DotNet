using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace TypedRest
{
    /// <summary>
    /// Detects links in HTTP responses.
    /// </summary>
    public interface ILinkHandler
    {
        /// <summary>
        /// Detects links in the HTTP response.
        /// </summary>
        /// <returns>Links grouped by relation type and link templates grouped by relation type.</returns>
        Task<(LinkDictionary links, IDictionary<string, string> linkTemplates)> HandleAsync(HttpResponseMessage response);
    }
}
