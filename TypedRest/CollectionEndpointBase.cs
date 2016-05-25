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
        /// <param name="ensureTrailingSlashOnParentUri">If true, ensures a trailing slash on the parent uri.</param>
        protected CollectionEndpointBase(IEndpoint parent, Uri relativeUri, bool ensureTrailingSlashOnParentUri = false)
            : base(parent, relativeUri.EnsureTrailingSlash(), ensureTrailingSlashOnParentUri)
        {
            var keyProperty = typeof(TEntity).GetRuntimeProperties()
                .FirstOrDefault(x => x.GetMethod != null && x.GetCustomAttributes(typeof(KeyAttribute), inherit: true).Any());
            if (keyProperty != null) _keyGetMethod = keyProperty.GetMethod;
        }

        /// <summary>
        /// Creates a new element collection endpoint.
        /// </summary>
        /// <param name="parent">The parent endpoint containing this one.</param>
        /// <param name="relativeUri">The URI of this endpoint relative to the <paramref name="parent"/>'s. Missing trailing slash will be appended automatically.</param>
        /// <param name="ensureTrailingSlashOnParentUri">If true, ensures a trailing slash on the parent uri.</param>
        protected CollectionEndpointBase(IEndpoint parent, string relativeUri, bool ensureTrailingSlashOnParentUri = false)
            // Use this instead of base to ensure trailing slash gets appended for REST collection URIs
            : this(parent, new Uri(relativeUri, UriKind.Relative), ensureTrailingSlashOnParentUri)
        {
        }

        public abstract TElementEndpoint this[Uri relativeUri] { get; }

        /// <summary>
        /// The Link relation type used by the server to set the collection child element URI template.
        /// <c>null</c> to use a simple relative URI rather than a URI template.
        /// </summary>
        /// <seealso cref="ICollectionEndpoint{TEntity,TElementEndpoint}.this[string]"/>
        public string ChildTemplateRel { get; set; }

        public TElementEndpoint this[string key]
        {
            get
            {
                if (key == null) throw new ArgumentNullException(nameof(key));

                return this[(ChildTemplateRel == null)
                    ? new Uri(Uri, key)
                    : new Uri(Uri, LinkTemplate(ChildTemplateRel).Resolve(new Dictionary<string, object> {["id"] = key}))
                    ];
            }
        }

        public TElementEndpoint this[TEntity entity] => this[GetCollectionKey(entity)];

        /// <summary>
        /// Maps the <paramref name="entity"/> to an key usable by <see cref="ICollectionEndpoint{TEntity,TElementEndpoint}.this[string]"/>.
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
            var response = await HandleResponseAsync(HttpClient.GetAsync(Uri, cancellationToken)).NoContext();
            return await response.Content.ReadAsAsync<List<TEntity>>(cancellationToken).NoContext();
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