using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace TypedRest
{
    /// <summary>
    /// REST endpoint that represents an RPC-like action.
    /// </summary>
    public class ActionEndpoint : TriggerEndpointBase, IActionEndpoint
    {
        /// <summary>
        /// Creates a new action endpoint.
        /// </summary>
        /// <param name="parent">The parent endpoint containing this one.</param>
        /// <param name="relativeUri">The URI of this endpoint relative to the <paramref name="parent"/>'s.</param>
        public ActionEndpoint(IEndpoint parent, Uri relativeUri)
            : base(parent, relativeUri)
        {
        }

        /// <summary>
        /// Creates a new action endpoint.
        /// </summary>
        /// <param name="parent">The parent endpoint containing this one.</param>
        /// <param name="relativeUri">The URI of this endpoint relative to the <paramref name="parent"/>'s. Prefix <c>./</c> to append a trailing slash to the <paramref name="parent"/> URI if missing.</param>
        public ActionEndpoint(IEndpoint parent, string relativeUri)
            : base(parent, relativeUri)
        {
        }

        public Task TriggerAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            return HandleResponseAsync(HttpClient.SendAsync(new HttpRequestMessage(HttpMethod.Post, Uri),
                cancellationToken));
        }
    }

    /// <summary>
    /// REST endpoint that represents an RPC-like action which takes <typeparamref name="TEntity"/> as input.
    /// </summary>
    /// <typeparam name="TEntity">The type of entity the endpoint takes as input.</typeparam>
    public class ActionEndpoint<TEntity> : TriggerEndpointBase, IActionEndpoint<TEntity>
    {
        /// <summary>
        /// Creates a new action endpoint with a relative URI.
        /// </summary>
        /// <param name="parent">The parent endpoint containing this one.</param>
        /// <param name="relativeUri">The URI of this endpoint relative to the <paramref name="parent"/>'s.</param>
        public ActionEndpoint(IEndpoint parent, Uri relativeUri)
            : base(parent, relativeUri)
        {
        }

        /// <summary>
        /// Creates a new action endpoint with a relative URI.
        /// </summary>
        /// <param name="parent">The parent endpoint containing this one.</param>
        /// <param name="relativeUri">The URI of this endpoint relative to the <paramref name="parent"/>'s. Prefix <c>./</c> to append a trailing slash to the <paramref name="parent"/> URI if missing.</param>
        public ActionEndpoint(IEndpoint parent, string relativeUri)
            : base(parent, relativeUri)
        {
        }

        public Task TriggerAsync(TEntity entity, CancellationToken cancellationToken = new CancellationToken())
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            return HandleResponseAsync(HttpClient.PostAsync(Uri, entity, Serializer, cancellationToken));
        }
    }
}