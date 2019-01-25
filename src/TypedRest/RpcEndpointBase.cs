using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace TypedRest
{
    /// <summary>
    /// Base class for building REST RPC-like endpoints.
    /// </summary>
    public abstract class RpcEndpointBase : EndpointBase, IRpcEndpoint
    {
        /// <summary>
        /// Creates a new RPC endpoint with a relative URI.
        /// </summary>
        /// <param name="referrer">The endpoint used to navigate to this one.</param>
        /// <param name="relativeUri">The URI of this endpoint relative to the <paramref name="referrer"/>'s.</param>
        protected RpcEndpointBase(IEndpoint referrer, Uri relativeUri)
            : base(referrer, relativeUri)
        {}

        /// <summary>
        /// Creates a new RPC endpoint with a relative URI.
        /// </summary>
        /// <param name="referrer">The endpoint used to navigate to this one.</param>
        /// <param name="relativeUri">The URI of this endpoint relative to the <paramref name="referrer"/>'s. Prefix <c>./</c> to append a trailing slash to the <paramref name="referrer"/> URI if missing.</param>
        protected RpcEndpointBase(IEndpoint referrer, string relativeUri)
            : base(referrer, relativeUri)
        {}

        public Task ProbeAsync(CancellationToken cancellationToken = new CancellationToken())
            => HandleResponseAsync(HttpClient.OptionsAsync(Uri, cancellationToken));

        public bool? InvokeAllowed => IsMethodAllowed(HttpMethod.Post);
    }
}
