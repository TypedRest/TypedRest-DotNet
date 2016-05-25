using System;
using System.Collections.Generic;
using System.Net.Http;
using TypedRest.UriTemplates;

namespace TypedRest
{
    /// <summary>
    /// REST endpoint, i.e. a remote HTTP resource.
    /// </summary>
    public interface IEndpoint
    {
        /// <summary>
        /// The HTTP client used to communicate with the remote resource.
        /// </summary>
        HttpClient HttpClient { get; }

        /// <summary>
        /// The HTTP URI of the remote resource.
        /// </summary>
        Uri Uri { get; }

        /// <summary>
        /// Retrieves all links with a specific relation type cached from the last request.
        /// </summary>
        /// <param name="rel">The relation type of the links to look for.</param>
        /// <returns>The hrefs of the links resolved relative to this endpoint's URI.</returns>
        /// <exception cref="KeyNotFoundException">No link with the specified <paramref name="rel"/> could be found.</exception>
        IEnumerable<Uri> GetLinks(string rel);

        /// <summary>
        /// Retrieves all links (with titles) with a specific relation type cached from the last request.
        /// </summary>
        /// <param name="rel">The relation type of the links to look for.</param>
        /// <returns>A map of hrefs (resolved relative to this endpoint's URI) to titles (may be <c>null</c>).</returns>
        /// <exception cref="KeyNotFoundException">No link with the specified <paramref name="rel"/> could be found.</exception>
        IDictionary<Uri, string> GetLinksWithTitles(string rel);

        /// <summary>
        /// Retrieves a single link with a specific relation type.
        /// </summary>
        /// <param name="rel">The relation type of the link to look for.</param>
        /// <returns>The href of the link resolved relative to this endpoint's URI.</returns>
        /// <exception cref="KeyNotFoundException">No link with the specified <paramref name="rel"/> could be found.</exception>
        /// <remarks>Uses cached data from last response if possible. Tries lazy lookup with HTTP HEAD on cache miss.</remarks>
        Uri Link(string rel);

        /// <summary>
        /// Retrieves a link template with a specific relation type.
        /// </summary>
        /// <param name="rel">The relation type of the link template to look for.</param>
        /// <returns>The unresolved link template.</returns>
        /// <exception cref="KeyNotFoundException">No link template with the specified <paramref name="rel"/> could be found.</exception>
        /// <remarks>Uses cached data from last response if possible. Tries lazy lookup with HTTP HEAD on cache miss.</remarks>
        UriTemplate LinkTemplate(string rel);

        /// <summary>
        /// Retrieves a link template with a specific relation type and resolves it.
        /// </summary>
        /// <param name="rel">The relation type of the link template to look for.</param>
        /// <param name="variables">An object used to provide properties for resolving the tmplate</param>
        /// <returns>The href of the link resolved relative to this endpoint's URI.</returns>
        /// <exception cref="KeyNotFoundException">No link template with the specified <paramref name="rel"/> could be found.</exception>
        /// <remarks>Uses cached data from last response if possible. Tries lazy lookup with HTTP HEAD on cache miss.</remarks>
        Uri LinkTemplate(string rel, object variables);
    }
}