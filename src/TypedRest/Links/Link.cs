namespace TypedRest.Links
{
    /// <summary>
    /// Represents a link to another resource.
    /// </summary>
    public sealed class Link
    {
        /// <summary>
        /// The relation type of the link.
        /// </summary>
        public string Rel { get; }

        /// <summary>
        /// The href/target of the link.
        /// </summary>
        public string Href { get; }

        /// <summary>
        /// The title of the link (optional).
        /// </summary>
        public string? Title { get; }

        /// <summary>
        /// Indicates whether the link is an URI Template (RFC 6570).
        /// </summary>
        public bool Templated { get; }

        /// <summary>
        /// Creates a new link
        /// </summary>
        /// <param name="rel">The relation type of the link.</param>
        /// <param name="href">The href/target of the link.</param>
        /// <param name="title">The title of the link (optional).</param>
        /// <param name="templated">Indicates whether the link is an URI Template (RFC 6570).</param>
        public Link(string rel, string href, string? title = null, bool templated = false)
        {
            Rel = rel;
            Href = href;
            Title = title;
            Templated = templated;
        }
    }
}
