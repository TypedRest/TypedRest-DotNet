using System;
using System.Net.Http;
using System.Threading;
using MorseCode.ITask;
using TypedRest.Http;

namespace TypedRest.Endpoints.Rpc
{
    /// <summary>
    /// RPC endpoint that returns <typeparamref name="TResult"/> as output when invoked.
    /// </summary>
    /// <typeparam name="TResult">The type of entity the endpoint returns as output.</typeparam>
    public class ProducerEndpoint<TResult> : RpcEndpointBase, IProducerEndpoint<TResult>
    {
        /// <summary>
        /// Creates a new function endpoint with a relative URI.
        /// </summary>
        /// <param name="referrer">The endpoint used to navigate to this one.</param>
        /// <param name="relativeUri">The URI of this endpoint relative to the <paramref name="referrer"/>'s.</param>
        public ProducerEndpoint(IEndpoint referrer, Uri relativeUri)
            : base(referrer, relativeUri)
        {}

        /// <summary>
        /// Creates a new function endpoint with a relative URI.
        /// </summary>
        /// <param name="referrer">The endpoint used to navigate to this one.</param>
        /// <param name="relativeUri">The URI of this endpoint relative to the <paramref name="referrer"/>'s. Add a <c>./</c> prefix here to imply a trailing slash <paramref name="referrer"/>'s URI.</param>
        public ProducerEndpoint(IEndpoint referrer, string relativeUri)
            : base(referrer, relativeUri)
        {}

        public async ITask<TResult> InvokeAsync(CancellationToken cancellationToken = default)
        {
            var response = await HandleAsync(() => HttpClient.SendAsync(new HttpRequestMessage(HttpMethod.Post, Uri), cancellationToken));

            return await response.Content.ReadAsAsync<TResult>(Serializer, cancellationToken);
        }
    }
}
