using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;

namespace TypedRest.Http
{
    /// <summary>
    /// Provides extension methods for <see cref="HttpResponseHeaders"/>.
    /// </summary>
    public static class HttpResponseHeadersExtensions
    {
        private static readonly Regex _regexHeaderLinks = new Regex("(<[^>]+>;[^,]+)", RegexOptions.Compiled);

        /// <summary>
        /// Returns all Link headers listed in an <see cref="HttpResponseHeaders"/> collection.
        /// </summary>
        public static IEnumerable<LinkHeader> GetLinkHeaders(this HttpResponseHeaders headers)
            // ReSharper disable RedundantEnumerableCastCall
            => headers.Where(x => x.Key.Equals("Link"))
                      .SelectMany(x => x.Value)
                      .SelectMany(x => _regexHeaderLinks.Matches(x).Cast<Match>())
                      .Where(x => x.Success)
                      .Select(x => new LinkHeader(x.Groups.Cast<Group>().Skip(1).Single().Value));
    }
}
