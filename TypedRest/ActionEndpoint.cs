using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace TypedRest
{
    /// <summary>
    /// REST endpoint that represents a single RPC-like action.
    /// </summary>
    public class ActionEndpoint : TriggerEndpointBase, IActionEndpoint
    {
        /// <summary>
        /// Creates a new action endpoint.
        /// </summary>
        /// <param name="parent">The parent endpoint containing this one.</param>
        /// <param name="relativeUri">The URI of this endpoint relative to the <paramref name="parent"/>'s.</param>
        /// <param name="ensureTrailingSlashOnParentUri">If true, ensures a trailing slash on the parent uri.</param>
        public ActionEndpoint(IEndpoint parent, Uri relativeUri, bool ensureTrailingSlashOnParentUri = false)
            : base(parent, relativeUri, ensureTrailingSlashOnParentUri)
        {
        }

        /// <summary>
        /// Creates a new action endpoint.
        /// </summary>
        /// <param name="parent">The parent endpoint containing this one.</param>
        /// <param name="relativeUri">The URI of this endpoint relative to the <paramref name="parent"/>'s.</param>
        /// <param name="ensureTrailingSlashOnParentUri">If true, ensures a trailing slash on the parent uri.</param>
        public ActionEndpoint(IEndpoint parent, string relativeUri, bool ensureTrailingSlashOnParentUri = false)
            : base(parent, relativeUri, ensureTrailingSlashOnParentUri)
        {
        }

        public Task TriggerAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            return HandleResponseAsync(HttpClient.SendAsync(new HttpRequestMessage(HttpMethod.Post, Uri),
                cancellationToken));
        }
    }

    /// <summary>
    /// REST endpoint that represents a single RPC-like action with <typeparamref name="TEntity"/> as input.
    /// </summary>
    public class ActionEndpoint<TEntity> : TriggerEndpointBase, IActionEndpoint<TEntity>
    {
        /// <summary>
        /// Creates a new action endpoint with a relative URI.
        /// </summary>
        /// <param name="parent">The parent endpoint containing this one.</param>
        /// <param name="relativeUri">The URI of this endpoint relative to the <paramref name="parent"/>'s.</param>
        /// <param name="ensureTrailingSlashOnParentUri">If true, ensures a trailing slash on the parent uri.</param>
        public ActionEndpoint(IEndpoint parent, Uri relativeUri, bool ensureTrailingSlashOnParentUri = false)
            : base(parent, relativeUri, ensureTrailingSlashOnParentUri)
        {
        }

        /// <summary>
        /// Creates a new action endpoint with a relative URI.
        /// </summary>
        /// <param name="parent">The parent endpoint containing this one.</param>
        /// <param name="relativeUri">The URI of this endpoint relative to the <paramref name="parent"/>'s.</param>
        /// <param name="ensureTrailingSlashOnParentUri">If true, ensures a trailing slash on the parent uri.</param>
        public ActionEndpoint(IEndpoint parent, string relativeUri, bool ensureTrailingSlashOnParentUri = false)
            : base(parent, relativeUri, ensureTrailingSlashOnParentUri)
        {
        }

        public Task TriggerAsync(TEntity entity, CancellationToken cancellationToken = new CancellationToken())
        {
            return HandleResponseAsync(HttpClient.PostAsync(Uri, entity, Serializer, cancellationToken));
        }
    }
}