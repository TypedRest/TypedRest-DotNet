using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Formatting;
using TypedRest.UriTemplates;

namespace TypedRest
{
    /// <summary>
    /// Endpoint, i.e. a remote HTTP resource.
    /// </summary>
    public interface IEndpoint
    {
        /// <summary>
        /// The HTTP URI of the remote resource.
        /// </summary>
        Uri Uri { get; }

        /// <summary>
        /// The HTTP client used to communicate with the remote resource.
        /// </summary>
        HttpClient HttpClient { get; }

        /// <summary>
        /// Controls the serialization of entities sent to and received from the server.
        /// </summary>
        MediaTypeFormatter Serializer { get; }

        /// <summary>
        /// Handles errors in HTTP responses.
        /// </summary>
        IErrorHandler ErrorHandler { get; }

        /// <summary>
        /// Detects links in HTTP responses.
        /// </summary>
        ILinkHandler LinkHandler { get; }

        /// <summary>
        /// Retrieves all links with a specific relation type cached from the last request.
        /// </summary>
        /// <param name="rel">The relation type of the links to look for.</param>
        IEnumerable<Link> GetLinks(string rel);

        /// <summary>
        /// Retrieves the href of a single link with a specific relation type.
        /// </summary>
        /// <param name="rel">The relation type of the link to look for.</param>
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
        /// <param name="variables">Variables for resolving the template</param>
        /// <returns>The href of the link resolved relative to this endpoint's URI.</returns>
        /// <exception cref="KeyNotFoundException">No link template with the specified <paramref name="rel"/> could be found.</exception>
        /// <remarks>Uses cached data from last response if possible. Tries lazy lookup with HTTP HEAD on cache miss.</remarks>
        Uri LinkTemplate(string rel, IDictionary<string, string> variables);

        /// <summary>
        /// Retrieves a link template with a specific relation type and resolves it.
        /// </summary>
        /// <param name="rel">The relation type of the link template to look for.</param>
        /// <param name="variables">An object used to provide properties for resolving the template</param>
        /// <returns>The href of the link resolved relative to this endpoint's URI.</returns>
        /// <exception cref="KeyNotFoundException">No link template with the specified <paramref name="rel"/> could be found.</exception>
        /// <remarks>Uses cached data from last response if possible. Tries lazy lookup with HTTP HEAD on cache miss.</remarks>
        Uri LinkTemplate(string rel, object variables);
    }
}
