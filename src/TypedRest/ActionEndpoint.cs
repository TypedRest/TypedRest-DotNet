using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace TypedRest
{
    /// <summary>
    /// REST endpoint that represents an RPC-like action.
    /// </summary>
    public class ActionEndpoint : RpcEndpointBase, IActionEndpoint
    {
        /// <summary>
        /// Creates a new action endpoint.
        /// </summary>
        /// <param name="referrer">The endpoint used to navigate to this one.</param>
        /// <param name="relativeUri">The URI of this endpoint relative to the <paramref name="referrer"/>'s.</param>
        public ActionEndpoint(IEndpoint referrer, Uri relativeUri)
            : base(referrer, relativeUri)
        {}

        /// <summary>
        /// Creates a new action endpoint.
        /// </summary>
        /// <param name="referrer">The endpoint used to navigate to this one.</param>
        /// <param name="relativeUri">The URI of this endpoint relative to the <paramref name="referrer"/>'s. Prefix <c>./</c> to append a trailing slash to the <paramref name="referrer"/> URI if missing.</param>
        public ActionEndpoint(IEndpoint referrer, string relativeUri)
            : base(referrer, relativeUri)
        {}

        public Task InvokeAsync(CancellationToken cancellationToken = new CancellationToken())
            => HandleResponseAsync(HttpClient.SendAsync(new HttpRequestMessage(HttpMethod.Post, Uri), cancellationToken));
    }
}
