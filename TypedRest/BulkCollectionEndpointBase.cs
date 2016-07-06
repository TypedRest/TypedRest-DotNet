using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace TypedRest
{
    /// <summary>
    /// Base class for building REST endpoints that represents a collection of <typeparamref name="TEntity"/>s as <typeparamref name="TElementEndpoint"/>s with bulk create and replace support.
    /// </summary>
    /// <typeparam name="TEntity">The type of entity the endpoint represents.</typeparam>
    /// <typeparam name="TElementEndpoint">The specific type of <see cref="IElementEndpoint{TEntity}"/> to provide for individual <typeparamref name="TEntity"/>s.</typeparam>
    public abstract class BulkCollectionEndpointBase<TEntity, TElementEndpoint> : CollectionEndpointBase<TEntity, TElementEndpoint>, IBulkCollectionEndpoint<TEntity, TElementEndpoint>
        where TElementEndpoint : class, IElementEndpoint<TEntity>
    {
        /// <summary>
        /// Creates a new bulk collection endpoint.
        /// </summary>
        /// <param name="parent">The parent endpoint containing this one.</param>
        /// <param name="relativeUri">The URI of this endpoint relative to the <paramref name="parent"/>'s. Missing trailing slash will be appended automatically.</param>
        protected BulkCollectionEndpointBase(IEndpoint parent, Uri relativeUri)
            : base(parent, relativeUri)
        {
            SetDefaultLink(rel: "bulk", hrefs: "bulk");
        }

        /// <summary>
        /// Creates a new bulk collection endpoint.
        /// </summary>
        /// <param name="parent">The parent endpoint containing this one.</param>
        /// <param name="relativeUri">The URI of this endpoint relative to the <paramref name="parent"/>'s. Missing trailing slash will be appended automatically. Prefix <c>./</c> to append a trailing slash to the <paramref name="parent"/> URI if missing.</param>
        protected BulkCollectionEndpointBase(IEndpoint parent, string relativeUri)
            : base(parent, relativeUri)
        {
            SetDefaultLink(rel: "bulk", hrefs: "bulk");
        }

        public bool? SetAllAllowed => IsVerbAllowed(HttpMethod.Put.Method);

        public Task SetAllAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = new CancellationToken())
        {
            if (entities == null) throw new ArgumentNullException(nameof(entities));

            return HandleResponseAsync(HttpClient.PutAsync(Uri, entities, Serializer, cancellationToken));
        }

        public virtual Task CreateAsync(IEnumerable<TEntity> entities,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (entities == null) throw new ArgumentNullException(nameof(entities));

            return HandleResponseAsync(HttpClient.PostAsync(Link("bulk"), entities, Serializer, cancellationToken));
        }
    }
}