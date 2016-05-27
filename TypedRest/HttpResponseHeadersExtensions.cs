using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;

namespace TypedRest
{
    /// <summary>
    /// Provides extension methods for <seealso cref="HttpResponseHeaders"/>.
    /// </summary>
    public static class HttpResponseHeadersExtensions
    {
        private static readonly Regex RegexHeaderLinks = new Regex("(<[^>]+>;[^,]+)", RegexOptions.Compiled);

        /// <summary>
        /// Returns all Link headers listed in an <see cref="HttpResponseHeaders"/> collection.
        /// </summary>
        public static IEnumerable<LinkHeader> GetLinkHeaders(this HttpResponseHeaders headers)
        {
            return headers.Where(x => x.Key.Equals("Link"))
                .SelectMany(x => x.Value)
                .SelectMany(x => RegexHeaderLinks.Matches(x).Cast<Match>())
                .Where(x => x.Success)
                .Select(x => new LinkHeader(x.Groups.Cast<Group>().Skip(1).Single().Value));
        }
    }
}