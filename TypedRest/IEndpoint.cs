using System;
using System.Collections.Generic;
using System.Net.Http;

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
        /// Retrieves a link from an HTTP Link header with a specific relation type. May be cached from a previous request or may be lazily requested.
        /// </summary>
        /// <param name="rel">The relation type of the link to look for.</param>
        /// <returns>The href of the link resolved relative to this endpoint's URI.</returns>
        /// <exception cref="KeyNotFoundException">No link with the specified <paramref name="rel"/> could be found.</exception>
        Uri Link(string rel);

        /// <summary>
        /// A set of <see cref="Uri"/>s of other <see cref="IEndpoint"/>s that may change to reflect operations performed on this endpoint.
        /// </summary>
        IReadOnlyCollection<Uri> NotifyTargets { get; }
    }
}