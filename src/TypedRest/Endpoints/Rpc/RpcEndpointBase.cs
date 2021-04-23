using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using TypedRest.Http;

namespace TypedRest.Endpoints.Rpc
{
    /// <summary>
    /// Base class for building RPC endpoints.
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
        /// <param name="relativeUri">The URI of this endpoint relative to the <paramref name="referrer"/>'s. Add a <c>./</c> prefix here to imply a trailing slash <paramref name="referrer"/>'s URI.</param>
        protected RpcEndpointBase(IEndpoint referrer, string relativeUri)
            : base(referrer, relativeUri)
        {}

        public async Task ProbeAsync(CancellationToken cancellationToken = default)
            => await HandleAsync(() => HttpClient.OptionsAsync(Uri, cancellationToken)).NoContext();

        public bool? InvokeAllowed => IsMethodAllowed(HttpMethod.Post);
    }
}
