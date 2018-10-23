using System.Collections.Generic;

namespace TypedRest
{
    /// <summary>
    /// Maps relation types to sets of links.
    /// </summary>
    public class LinkDictionary : Dictionary<string, ISet<Link>>
    {
        /// <summary>
        /// Adds a link to the list.
        /// </summary>
        /// <param name="rel">The relation type of the link.</param>
        /// <param name="link">The link to add.</param>
        public void Add(string rel, Link link)
        {
            if (!TryGetValue(rel, out var links))
                Add(rel, links = new HashSet<Link>());

            links.Remove(link); // overwrite existing
            links.Add(link);
        }
    }
}
