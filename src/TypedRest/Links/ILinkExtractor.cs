namespace TypedRest.Links;

/// <summary>
/// Extracts links from responses.
/// </summary>
public interface ILinkExtractor
{
    /// <summary>
    /// Extracts links from the <paramref name="response"/>.
    /// </summary>
    /// <exception cref="FormatException">One or more of the links found are invalid (e.g. missing a 'rel' type).</exception>
    Task<IReadOnlyList<Link>> GetLinksAsync(HttpResponseMessage response);
}