using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace TypedRest
{
    /// <summary>
    /// Base class for building REST endpoints that represents a collection of <typeparamref name="TEntity"/>s as <typeparamref name="TElementEndpoint"/>s.
    /// </summary>
    /// <typeparam name="TEntity">The type of entity the endpoint represents.</typeparam>
    /// <typeparam name="TElementEndpoint">The specific type of <see cref="IElementEndpoint{TEntity}"/> to provide for individual <typeparamref name="TEntity"/>s.</typeparam>
    public abstract class CollectionEndpointBase<TEntity, TElementEndpoint> : EndpointBase, ICollectionEndpoint<TEntity, TElementEndpoint>
        where TElementEndpoint : class, IElementEndpoint<TEntity>
    {
        private readonly MethodInfo _keyGetMethod;

        /// <summary>
        /// Creates a new element collection endpoint.
        /// </summary>
        /// <param name="parent">The parent endpoint containing this one.</param>
        /// <param name="relativeUri">The URI of this endpoint relative to the <paramref name="parent"/>'s. Missing trailing slash will be appended automatically.</param>
        protected CollectionEndpointBase(IEndpoint parent, Uri relativeUri)
            : base(parent, relativeUri.EnsureTrailingSlash())
        {
            var keyProperty = typeof(TEntity).GetPublicProperties().FirstOrDefault(x => x.HasAttribute<KeyAttribute>());
            if (keyProperty != null) _keyGetMethod = keyProperty.GetMethod;
        }

        /// <summary>
        /// Creates a new element collection endpoint.
        /// </summary>
        /// <param name="parent">The parent endpoint containing this one.</param>
        /// <param name="relativeUri">The URI of this endpoint relative to the <paramref name="parent"/>'s. Missing trailing slash will be appended automatically.</param>
        protected CollectionEndpointBase(IEndpoint parent, string relativeUri)
            // Use this instead of base to ensure trailing slash gets appended for REST collection URIs
            : this(parent, new Uri(relativeUri, UriKind.Relative))
        {
        }

        public abstract TElementEndpoint this[Uri relativeUri] { get; }

        public TElementEndpoint this[TEntity entity] => this[new Uri(GetCollectionKey(entity), UriKind.Relative)];

        /// <summary>
        /// Maps the <paramref name="entity"/> to an key usable by <see cref="ICollectionEndpoint{TEntity,TElementEndpoint}.this[Uri]"/>.
        /// </summary>
        protected virtual string GetCollectionKey(TEntity entity)
        {
            if (_keyGetMethod == null)
                throw new InvalidOperationException(typeof (TElementEndpoint).Name + " has no property marked with [Key] attribute.");
            return _keyGetMethod.Invoke(entity, null).ToString();
        }

        public virtual async Task<ICollection<TEntity>> ReadAllAsync(
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var response = await HandleResponseAsync(HttpClient.GetAsync(Uri, cancellationToken));
            return await response.Content.ReadAsAsync<List<TEntity>>(cancellationToken);
        }

        public virtual async Task<TElementEndpoint> CreateAsync(TEntity entity,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var response = await HandleResponseAsync(HttpClient.PostAsync(Uri, entity, Serializer, cancellationToken));

            return (response.StatusCode == HttpStatusCode.Created)
                ? this[response.Headers.Location]
                : null;
        }
    }
}