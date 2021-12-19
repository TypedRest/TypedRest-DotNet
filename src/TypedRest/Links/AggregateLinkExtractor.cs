namespace TypedRest.Links
{
    /// <summary>
    /// Combines the results of multiple <see cref="ILinkExtractor"/>s.
    /// </summary>
    public class AggregateLinkExtractor : ILinkExtractor
    {
        private readonly ILinkExtractor[] _extractors;

        /// <summary>
        /// Creates a new aggregate link extractor.
        /// </summary>
        /// <param name="extractors">The link extractors to aggregate.</param>
        public AggregateLinkExtractor(params ILinkExtractor[] extractors)
        {
            _extractors = extractors;
        }

        public async Task<IReadOnlyList<Link>> GetLinksAsync(HttpResponseMessage response)
        {
            var result = new List<Link>();
            foreach (var extractor in _extractors)
                result.AddRange(await extractor.GetLinksAsync(response));
            return result;
        }
    }
}
