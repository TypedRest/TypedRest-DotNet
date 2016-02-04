using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace TypedRest
{
    /// <summary>
    /// REST endpoint that represents a single triggerable action.
    /// </summary>
    public class TriggerEndpoint : EndpointBase, ITriggerEndpoint
    {
        /// <summary>
        /// Creates a new trigger endpoint.
        /// </summary>
        /// <param name="parent">The parent endpoint containing this one.</param>
        /// <param name="relativeUri">The URI of this endpoint relative to the <paramref name="parent"/>'s.</param>
        public TriggerEndpoint(IEndpoint parent, Uri relativeUri) : base(parent, relativeUri)
        {
        }

        /// <summary>
        /// Creates a new trigger endpoint.
        /// </summary>
        /// <param name="parent">The parent endpoint containing this one.</param>
        /// <param name="relativeUri">The URI of this endpoint relative to the <paramref name="parent"/>'s.</param>
        public TriggerEndpoint(IEndpoint parent, string relativeUri) : base(parent, relativeUri)
        {
        }

        public Task ProbeAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            return HandleResponseAsync(HttpClient.OptionsAsync(Uri, cancellationToken));
        }

        public bool? TriggerAllowed => IsVerbAllowed(HttpMethod.Post.Method);

        public Task TriggerAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            return HandleResponseAsync(HttpClient.SendAsync(new HttpRequestMessage(HttpMethod.Post, Uri),
                cancellationToken));
        }
    }
}