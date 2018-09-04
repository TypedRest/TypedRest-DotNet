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
    /// REST endpoint that represents a collection of <typeparamref name="TEntity"/>s as <typeparamref name="TElementEndpoint"/>s.
    /// </summary>
    /// <typeparam name="TEntity">The type of entity the endpoint represents.</typeparam>
    /// <typeparam name="TElementEndpoint">The type of <see cref="IEndpoint"/> to provide for individual <typeparamref name="TEntity"/>s. This must be a non-abstract class with a constructor that takes an <see cref="IEndpoint"/> and an <see cref="Uri"/>, unless you override <see cref="BuildElementEndpoint"/>.</typeparam>
    public class CollectionEndpoint<TEntity, TElementEndpoint> : ETagEndpointBase, ICollectionEndpoint<TEntity, TElementEndpoint>
        where TElementEndpoint : class, IEndpoint
    {
        /// <summary>
        /// Creates a new element collection endpoint.
        /// </summary>
        /// <param name="referrer">The endpoint used to navigate to this one.</param>
        /// <param name="relativeUri">The URI of this endpoint relative to the <paramref name="referrer"/>'s.</param>
        public CollectionEndpoint(IEndpoint referrer, Uri relativeUri)
            : base(referrer, relativeUri)
        {
            SetupChildHandling();
        }

        /// <summary>
        /// Creates a new element collection endpoint.
        /// </summary>
        /// <param name="referrer">The endpoint used to navigate to this one.</param>
        /// <param name="relativeUri">The URI of this endpoint relative to the <paramref name="referrer"/>'s. Prefix <c>./</c> to append a trailing slash to the <paramref name="referrer"/> URI if missing.</param>
        public CollectionEndpoint(IEndpoint referrer, string relativeUri)
            : base(referrer, relativeUri)
        {
            SetupChildHandling();
        }

        private MethodInfo _getIdMethod;

        private void SetupChildHandling()
        {
            var idProperty = typeof(TEntity).GetTypeInfo()
                                            .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                                            .FirstOrDefault(x => x.GetMethod != null
                                                              && x.GetCustomAttribute<KeyAttribute>(inherit: true) != null);

            if (idProperty != null) _getIdMethod = idProperty.GetMethod;

            SetDefaultLinkTemplate(rel: "child", href: "./{id}");
        }

        /// <summary>
        /// Builds a <typeparamref name="TElementEndpoint"/> for a specific child element of this collection. Does not perform any network traffic yet.
        /// </summary>
        /// <param name="relativeUri">The URI of the child endpoint relative to the this endpoint.</param>
        protected virtual TElementEndpoint BuildElementEndpoint(Uri relativeUri) => (TElementEndpoint)Activator.CreateInstance(typeof(TElementEndpoint), this, relativeUri);

        public virtual TElementEndpoint this[string id]
        {
            get
            {
                if (id == null) throw new ArgumentNullException(nameof(id));

                return BuildElementEndpoint(LinkTemplate("child", new {id}));
            }
        }

        public virtual TElementEndpoint this[TEntity entity]
        {
            get
            {
                if (_getIdMethod == null)
                    throw new InvalidOperationException($"{typeof(TEntity).Name} has no property marked with [Key] attribute.");
                string id = _getIdMethod.Invoke(entity, null).ToString();
                return this[id];
            }
        }

        public bool? ReadAllAllowed => IsMethodAllowed(HttpMethod.Get);

        public virtual async Task<List<TEntity>> ReadAllAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            var content = await GetContentAsync(cancellationToken);
            return await content.ReadAsAsync<List<TEntity>>(new[] {Serializer}, cancellationToken).NoContext();
        }

        /// <summary>
        /// The value used for <see cref="RangeHeaderValue.Unit"/>.
        /// </summary>
        public string RangeUnit { get; set; } = "elements";

        protected override void HandleCapabilities(HttpResponseMessage response)
        {
            base.HandleCapabilities(response);
            ReadRangeAllowed = response.Headers.AcceptRanges.Contains(RangeUnit);
        }

        public bool? ReadRangeAllowed { get; private set; }

        public async Task<PartialResponse<TEntity>> ReadRangeAsync(RangeItemHeaderValue range,
                                                                   CancellationToken cancellationToken = default(CancellationToken))
        {
            var request = new HttpRequestMessage(HttpMethod.Get, Uri)
            {
                Headers = {Range = new RangeHeaderValue {Ranges = {range}, Unit = RangeUnit}}
            };

            var response = await HandleResponseAsync(HttpClient.SendAsync(request, cancellationToken)).NoContext();
            return new PartialResponse<TEntity>(
                elements: await response.Content.ReadAsAsync<List<TEntity>>(new[] {Serializer}, cancellationToken).NoContext(),
                range: response.Content.Headers.ContentRange);
        }

        public bool? CreateAllowed => IsMethodAllowed(HttpMethod.Post);

        public virtual async Task<TElementEndpoint> CreateAsync(TEntity entity,
                                                                CancellationToken cancellationToken = default(CancellationToken))
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            var response = await HandleResponseAsync(HttpClient.PostAsync(Uri, entity, Serializer, cancellationToken)).NoContext();

            return (response.StatusCode == HttpStatusCode.Created || response.StatusCode == HttpStatusCode.Accepted) && (response.Headers.Location != null)
                ? BuildElementEndpoint(response.Headers.Location)
                : null;
        }

        public bool? CreateAllAllowed => IsMethodAllowed(HttpClientExtensions.Patch);

        public virtual Task CreateAllAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (entities == null) throw new ArgumentNullException(nameof(entities));

            return HandleResponseAsync(HttpClient.PatchAsync(Uri, entities, Serializer, cancellationToken));
        }

        public bool? SetAllAllowed => IsMethodAllowed(HttpMethod.Put);

        public async Task SetAllAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = new CancellationToken())
        {
            if (entities == null) throw new ArgumentNullException(nameof(entities));

            var content = new ObjectContent<IEnumerable<TEntity>>(entities, Serializer);
            await PutContentAsync(content, cancellationToken);
        }
    }

    /// <summary>
    /// REST endpoint that represents a collection of <typeparamref name="TEntity"/>s as <see cref="ElementEndpoint{TEntity}"/>s.
    /// </summary>
    /// <typeparam name="TEntity">The type of entity the endpoint represents.</typeparam>
    public class CollectionEndpoint<TEntity> : CollectionEndpoint<TEntity, IElementEndpoint<TEntity>>, ICollectionEndpoint<TEntity>
    {
        /// <summary>
        /// Creates a new collection endpoint.
        /// </summary>
        /// <param name="referrer">The endpoint used to navigate to this one.</param>
        /// <param name="relativeUri">The URI of this endpoint relative to the <paramref name="referrer"/>'s.</param>
        public CollectionEndpoint(IEndpoint referrer, Uri relativeUri)
            : base(referrer, relativeUri)
        {}

        /// <summary>
        /// Creates a new collection endpoint.
        /// </summary>
        /// <param name="referrer">The endpoint used to navigate to this one.</param>
        /// <param name="relativeUri">The URI of this endpoint relative to the <paramref name="referrer"/>'s. Prefix <c>./</c> to append a trailing slash to the <paramref name="referrer"/> URI if missing.</param>
        public CollectionEndpoint(IEndpoint referrer, string relativeUri)
            : base(referrer, relativeUri)
        {}

        protected override IElementEndpoint<TEntity> BuildElementEndpoint(Uri relativeUri) => new ElementEndpoint<TEntity>(this, relativeUri);
    }
}
