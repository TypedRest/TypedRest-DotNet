using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TypedRest.Links
{
    /// <summary>
    /// Extracts links from HTTP headers.
    /// </summary>
    public class HeaderLinkExtractor : ILinkExtractor
    {
        // ReSharper disable RedundantEnumerableCastCall

        private static readonly Regex _regexHeaderLinks = new Regex("(<[^>]+>;[^,]+)", RegexOptions.Compiled);

        public Task<IReadOnlyList<Link>> GetLinksAsync(HttpResponseMessage response)
            => Task.FromResult<IReadOnlyList<Link>>(
                response.Headers
                        .Where(x => x.Key.Equals("Link"))
                        .SelectMany(x => x.Value)
                        .SelectMany(x => _regexHeaderLinks.Matches(x).Cast<Match>())
                        .Where(x => x.Success)
                        .Select(x => ParseLink(x.Groups.Cast<Group>().Skip(1).Single().Value))
                        .ToList());

        private static readonly Regex _regexLinkFields =
            new Regex("(?=^<(?'href'[^>]*)>)|(?'field'[a-z]+)=\"?(?'value'[^\",;]*)\"?", RegexOptions.Compiled);

        private static Link ParseLink(string value)
        {
            string? href = null, rel = null, title = null;
            bool templated = false;

            foreach (var match in _regexLinkFields.Matches(value).Cast<Match>())
            {
                href ??= match.Groups["href"].Value;

                if (match.Groups["field"].Success)
                {
                    if (rel == null && match.Groups["field"].Value.Equals("rel", StringComparison.OrdinalIgnoreCase))
                        rel = match.Groups["value"].Value;

                    if (match.Groups["field"].Value.Equals("templated", StringComparison.OrdinalIgnoreCase))
                        templated = match.Groups["value"].Value.Equals("true", StringComparison.OrdinalIgnoreCase);

                    if (title == null && match.Groups["field"].Value.Equals("title", StringComparison.OrdinalIgnoreCase))
                        title = match.Groups["value"].Value;
                }
            }

            return new Link(
                rel ?? throw new FormatException("The link header is lacking the mandatory 'rel' field."),
                href ?? throw new FormatException("The link header is lacking the mandatory href."),
                title,
                templated);
        }
    }
}
