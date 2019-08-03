using System;

namespace TypedRest.Links
{
    /// <summary>
    /// Represents a link to an HTTP resource.
    /// </summary>
    public class Link : IEquatable<Link>
    {
        /// <summary>
        /// The href/target of the link.
        /// </summary>
        public Uri Href { get; }

        /// <summary>
        /// The title of the link (optional).
        /// </summary>
        public string Title { get; }

        /// <summary>
        /// Creates a new link.
        /// </summary>
        /// <param name="href">The href/target of the link.</param>
        /// <param name="title">The title of the link (optional).</param>
        public Link(Uri href, string title = null)
        {
            Href = href ?? throw new ArgumentNullException(nameof(href));
            Title = title;
        }

        public bool Equals(Link other)
            => other != null && Href.Equals(other.Href);

        public override bool Equals(object obj)
            => obj is Link other && Equals(other);

        public override int GetHashCode()
            => Href.GetHashCode();
    }
}
