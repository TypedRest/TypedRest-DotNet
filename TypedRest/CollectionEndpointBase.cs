using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
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
    public abstract class CollectionEndpointBase<TEntity, TElementEndpoint> : ETagEndpointBase, ICollectionEndpoint<TEntity, TElementEndpoint>
        where TElementEndpoint : class, IElementEndpoint<TEntity>
    {
        /// <summary>
        /// Creates a new element collection endpoint.
        /// </summary>
        /// <param name="referrer">The endpoint used to navigate to this one.</param>
        /// <param name="relativeUri">The URI of this endpoint relative to the <paramref name="referrer"/>'s. Missing trailing slash will be appended automatically.</param>
        protected CollectionEndpointBase(IEndpoint referrer, Uri relativeUri)
            : base(referrer, relativeUri.EnsureTrailingSlash())
        {
            SetupChildHandling();
        }

        /// <summary>
        /// Creates a new element collection endpoint.
        /// </summary>
        /// <param name="referrer">The endpoint used to navigate to this one.</param>
        /// <param name="relativeUri">The URI of this endpoint relative to the <paramref name="referrer"/>'s. Missing trailing slash will be appended automatically. Prefix <c>./</c> to append a trailing slash to the <paramref name="referrer"/> URI if missing.</param>
        protected CollectionEndpointBase(IEndpoint referrer, string relativeUri)
            : base(referrer, relativeUri.EndsWith("/") ? relativeUri : relativeUri + "/")
        {
            SetupChildHandling();
        }

        private MethodInfo _getIdMethod;

        private void SetupChildHandling()
        {
            var idProperty = typeof(TEntity).GetRuntimeProperties()
                .FirstOrDefault(x => x.GetMethod != null && x.GetCustomAttributes(typeof(KeyAttribute), inherit: true).Any());
            if (idProperty != null) _getIdMethod = idProperty.GetMethod;

            SetDefaultLinkTemplate(rel: "child", href: "{id}");
        }

        public abstract TElementEndpoint this[Uri relativeUri] { get; }

        public TElementEndpoint this[string id]
        {
            get
            {
                if (id == null) throw new ArgumentNullException(nameof(id));

                return this[LinkTemplate("child", new {id = id})];
            }
        }

        public TElementEndpoint this[TEntity entity] => this[GetCollectionId(entity)];

        /// <summary>
        /// Maps the <paramref name="entity"/> to an ID usable by <see cref="ICollectionEndpoint{TEntity,TElementEndpoint}.this[string]"/>.
        /// </summary>
        protected virtual string GetCollectionId(TEntity entity)
        {
            if (_getIdMethod == null)
                throw new InvalidOperationException(typeof (TElementEndpoint).Name + " has no property marked with [Key] attribute.");
            return _getIdMethod.Invoke(entity, null).ToString();
        }

        public virtual async Task<List<TEntity>> ReadAllAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            var content = await GetContentAsync(cancellationToken);
            return await content.ReadAsAsync<List<TEntity>>(new[] {Serializer}, cancellationToken).NoContext();
        }

        public bool? CreateAllowed => IsVerbAllowed(HttpMethod.Post.Method);

        public virtual async Task<TElementEndpoint> CreateAsync(TEntity entity,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            var response = await HandleResponseAsync(HttpClient.PostAsync(Uri, entity, Serializer, cancellationToken)).NoContext();

            return (response.StatusCode == HttpStatusCode.Created || response.StatusCode == HttpStatusCode.Accepted) && (response.Headers.Location != null)
                ? this[response.Headers.Location]
                : null;
        }
    }
}