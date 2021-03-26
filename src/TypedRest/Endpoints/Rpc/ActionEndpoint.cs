using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace TypedRest.Endpoints.Rpc
{
    /// <summary>
    /// RPC endpoint that is invoked with no input or output.
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
        /// <param name="relativeUri">The URI of this endpoint relative to the <paramref name="referrer"/>'s. Add a <c>./</c> prefix here to imply a trailing slash <paramref name="referrer"/>'s URI.</param>
        public ActionEndpoint(IEndpoint referrer, string relativeUri)
            : base(referrer, relativeUri)
        {}

        public Task InvokeAsync(CancellationToken cancellationToken = default)
            => HandleAsync(() => HttpClient.SendAsync(new(HttpMethod.Post, Uri), cancellationToken));
    }
}
