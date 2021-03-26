using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using MorseCode.ITask;
using TypedRest.Http;

namespace TypedRest.Endpoints.Generic
{
    /// <summary>
    /// Endpoint for a collection of <typeparamref name="TEntity"/>s addressable as <typeparamref name="TElementEndpoint"/>s.
    /// </summary>
    /// <typeparam name="TEntity">The type of individual elements in the collection.</typeparam>
    /// <typeparam name="TElementEndpoint">The type of <see cref="IElementEndpoint{TEntity}"/> to provide for individual <typeparamref name="TEntity"/>s. Must have a public constructor with an <see cref="IEndpoint"/> and an <see cref="Uri"/> or string parameter.</typeparam>
    public class CollectionEndpoint<TEntity, TElementEndpoint> : CachingEndpointBase, ICollectionEndpoint<TEntity, TElementEndpoint>
        where TEntity : class
        where TElementEndpoint : class, IElementEndpoint<TEntity>
    {
        /// <summary>
        /// Creates a new element collection endpoint.
        /// </summary>
        /// <param name="referrer">The endpoint used to navigate to this one.</param>
        /// <param name="relativeUri">The URI of this endpoint relative to the <paramref name="referrer"/>'s.</param>
        public CollectionEndpoint(IEndpoint referrer, Uri relativeUri)
            : base(referrer, relativeUri)
        {
            SetupElementHandling();
        }

        /// <summary>
        /// Creates a new element collection endpoint.
        /// </summary>
        /// <param name="referrer">The endpoint used to navigate to this one.</param>
        /// <param name="relativeUri">The URI of this endpoint relative to the <paramref name="referrer"/>'s. Add a <c>./</c> prefix here to imply a trailing slash <paramref name="referrer"/>'s URI.</param>
        public CollectionEndpoint(IEndpoint referrer, string relativeUri)
            : base(referrer, relativeUri)
        {
            SetupElementHandling();
        }

        private void SetupElementHandling()
        {
            SetDefaultLinkTemplate(rel: "child", href: "./{id}");
        }

        /// <summary>
        /// Instantiates a <typeparamref name="TElementEndpoint"/> with a referrer and a relative URI.
        /// </summary>
        private static readonly Func<IEndpoint, Uri, TElementEndpoint> _getElementEndpoint = GetConstructor<TElementEndpoint>();

        public virtual TElementEndpoint this[string id]
        {
            get
            {
                if (string.IsNullOrEmpty(id)) throw new ArgumentNullException(nameof(id));

                return _getElementEndpoint(this, LinkTemplate("child", new {id}));
            }
        }

        /// <summary>
        /// Gets the ID for a <typeparamref name="TEntity"/>. May be <c>null</c>.
        /// </summary>
        private static readonly Func<TEntity, object>? _getElementId = typeof(TEntity).GetPropertyWith<KeyAttribute>()?.GetMethod?.ToFunc<TEntity, object>();

        public virtual TElementEndpoint this[TEntity entity]
        {
            get
            {
                if (_getElementId == null)
                    throw new InvalidOperationException($"{typeof(TEntity).Name} has no property marked with [Key] attribute.");

                return this[_getElementId(entity).ToString()!];
            }
        }

        public bool? ReadAllAllowed => IsMethodAllowed(HttpMethod.Get);

        public virtual async Task<List<TEntity>> ReadAllAsync(CancellationToken cancellationToken = default)
        {
            var content = await GetContentAsync(cancellationToken);
            return await content.ReadAsAsync<List<TEntity>>(Serializer, cancellationToken).NoContext();
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

        public virtual async Task<PartialResponse<TEntity>> ReadRangeAsync(RangeItemHeaderValue range, CancellationToken cancellationToken = default)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, Uri)
            {
                Headers = {Range = new() {Ranges = {range}, Unit = RangeUnit}}
            };

            var response = await HandleAsync(() => HttpClient.SendAsync(request, cancellationToken)).NoContext();
            return new(
                elements: await response.Content.ReadAsAsync<List<TEntity>>(Serializer, cancellationToken).NoContext(),
                range: response.Content.Headers.ContentRange);
        }

        public bool? CreateAllowed => IsMethodAllowed(HttpMethod.Post);

        public virtual async ITask<TElementEndpoint?> CreateAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            var response = await HandleAsync(() => HttpClient.PostAsync(Uri, entity, Serializer, cancellationToken)).NoContext();

            TElementEndpoint elementEndpoint;
            if (response.Headers.Location == null)
            {
                try
                {
                    // Infer URL from entity ID in response body
                    elementEndpoint = this[await response.Content.ReadAsAsync<TEntity>(cancellationToken)];
                }
                catch
                {
                    // No element endpoint
                    return null;
                }
            }
            else
            {
                // Explicit element endpoint URL from "Location" header
                elementEndpoint = _getElementEndpoint(this, response.Headers.Location);
            }

            if (elementEndpoint is ICachingEndpoint caching)
                caching.ResponseCache = ResponseCache.From(response);

            return elementEndpoint;
        }

        public bool? CreateAllAllowed => IsMethodAllowed(HttpMethods.Patch);

        public virtual Task CreateAllAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
        {
            if (entities == null) throw new ArgumentNullException(nameof(entities));

            return HandleAsync(() => HttpClient.PatchAsync(Uri, entities, Serializer, cancellationToken));
        }

        public bool? SetAllAllowed => IsMethodAllowed(HttpMethod.Put);

        public async Task SetAllAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
        {
            if (entities == null) throw new ArgumentNullException(nameof(entities));

            var content = new ObjectContent<IEnumerable<TEntity>>(entities, Serializer);
            await PutContentAsync(content, cancellationToken);
        }
    }
}
