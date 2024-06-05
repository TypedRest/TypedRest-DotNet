using System.Text.RegularExpressions;

namespace TypedRest.Links;

/// <summary>
/// Extracts links from HTTP headers.
/// </summary>
public sealed class HeaderLinkExtractor : ILinkExtractor
{
    // ReSharper disable RedundantEnumerableCastCall

    private static readonly Regex _regexHeaderLinks = new("<[^>]*>\\s*(\\s*;\\s*[^\\(\\)<>@,;:\\\"\\/\\[\\]\\?={} \\t]+=(([^\\(\\)<>@,;:\\\"\\/\\[\\]\\?={} \\t]+)|(\\\"[^\\\"]*\\\")))*(,|$)", RegexOptions.Compiled);

    public Task<IReadOnlyList<Link>> GetLinksAsync(HttpResponseMessage response)
        => Task.FromResult<IReadOnlyList<Link>>(
            response.Headers
                    .Where(x => x.Key.Equals("Link"))
                    .SelectMany(x => x.Value)
                    .SelectMany(x => _regexHeaderLinks.Matches(x).Cast<Match>())
                    .Where(x => x.Success)
                    .Select(x => ParseLink(x.Groups.Cast<Group>().First().Value))
                    .ToList());

    private static readonly Regex _regexLinkFields = new("[^\\(\\)<>@,;:\"\\/\\[\\]\\?={} \\t]+=(([^\\(\\)<>@,;:\"\\/\\[\\]\\?={} \\t]+)|(\"[^\"]*\"))", RegexOptions.Compiled);

    private static Link ParseLink(string value)
    {
        var split = value.Split(['>'], 2);
        string href = split[0].Substring(1);
        string? rel = null, title = null;
        bool templated = false;

        foreach (string param in _regexLinkFields.Matches(split[1]).Cast<Match>().Select(x => x.Groups.Cast<Group>().First().Value))
        {
            var paramSplit = param.Split(['='], 2);
            if (paramSplit.Length != 2) continue;
            switch (paramSplit[0])
            {
                case "rel":
                    rel = paramSplit[1];
                    break;
                case "title":
                    title = paramSplit[1];
                    if (title.StartsWith("\"") && title.EndsWith("\""))
                        title = title.Substring(1, title.Length - 2);
                    break;
                case "templated" when paramSplit[1] == "true":
                    templated = true;
                    break;
            }
        }

        return new(
            rel ?? throw new FormatException("The link header is lacking the mandatory 'rel' field."),
            href,
            title,
            templated);
    }
}
