using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Formatting;
using TypedRest.Errors;
using TypedRest.Links;

namespace TypedRest.Endpoints
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
        /// Handles errors in responses.
        /// </summary>
        IErrorHandler ErrorHandler { get; }

        /// <summary>
        /// Extracts links from responses.
        /// </summary>
        ILinkExtractor LinkExtractor { get; }

        /// <summary>
        /// Resolves all links with a specific relation type. Uses cached data from last response.
        /// </summary>
        /// <param name="rel">The relation type of the links to look for.</param>
        IReadOnlyList<(Uri uri, string? title)> GetLinks(string rel);

        /// <summary>
        /// Resolves a single link with a specific relation type. Uses cached data from last response if possible. Tries lazy lookup with HTTP HEAD on cache miss.
        /// </summary>
        /// <param name="rel">The relation type of the link to look for.</param>
        /// <exception cref="KeyNotFoundException">No link with the specified <paramref name="rel"/> could be found.</exception>
        Uri Link(string rel);

        /// <summary>
        /// Resolves a link template with a specific relation type. Uses cached data from last response if possible. Tries lazy lookup with HTTP HEAD on cache miss.
        /// </summary>
        /// <param name="rel">The relation type of the link template to look for.</param>
        /// <param name="variables">Variables for resolving the template.</param>
        /// <returns>The href of the link resolved relative to this endpoint's URI.</returns>
        /// <exception cref="KeyNotFoundException">No link template with the specified <paramref name="rel"/> could be found.</exception>
        Uri LinkTemplate(string rel, IDictionary<string, object> variables);

        /// <summary>
        /// Resolves a link template with a specific relation type.
        /// </summary>
        /// <param name="rel">The relation type of the link template to look for.</param>
        /// <param name="variables">An object used to provide properties for resolving the template.</param>
        /// <returns>The href of the link resolved relative to this endpoint's URI.</returns>
        /// <exception cref="KeyNotFoundException">No link template with the specified <paramref name="rel"/> could be found.</exception>
        /// <remarks>Uses cached data from last response if possible. Tries lazy lookup with HTTP HEAD on cache miss.</remarks>
        Uri LinkTemplate(string rel, object variables);
    }
}
