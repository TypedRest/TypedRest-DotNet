using System;
using System.Net.Http;

namespace TypedRest
{
    /// <summary>
    /// REST endpoint, i.e. a remote HTTP resource.
    /// </summary>
    public interface IRestEndpoint
    {
        /// <summary>
        /// The HTTP client used to communicate with the remote item.
        /// </summary>
        HttpClient HttpClient { get; }

        /// <summary>
        /// The HTTP URI of the remote item.
        /// </summary>
        Uri Uri { get; }
    }
}