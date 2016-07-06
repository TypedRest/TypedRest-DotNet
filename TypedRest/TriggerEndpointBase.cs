using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace TypedRest
{
    /// <summary>
    /// Base class for building REST RPC-like endpoints.
    /// </summary>
    public abstract class TriggerEndpointBase : EndpointBase, ITriggerEndpoint
    {
        /// <summary>
        /// Creates a new trigger endpoint with a relative URI.
        /// </summary>
        /// <param name="parent">The parent endpoint containing this one.</param>
        /// <param name="relativeUri">The URI of this endpoint relative to the <paramref name="parent"/>'s.</param>
        protected TriggerEndpointBase(IEndpoint parent, Uri relativeUri)
            : base(parent, relativeUri)
        {
        }

        /// <summary>
        /// Creates a new trigger endpoint with a relative URI.
        /// </summary>
        /// <param name="parent">The parent endpoint containing this one.</param>
        /// <param name="relativeUri">The URI of this endpoint relative to the <paramref name="parent"/>'s. Prefix <c>./</c> to append a trailing slash to the <paramref name="parent"/> URI if missing.</param>
        protected TriggerEndpointBase(IEndpoint parent, string relativeUri)
            : base(parent, relativeUri)
        {
        }

        public Task ProbeAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            return HandleResponseAsync(HttpClient.OptionsAsync(Uri, cancellationToken));
        }

        public bool? TriggerAllowed => IsVerbAllowed(HttpMethod.Post.Method);
    }
}