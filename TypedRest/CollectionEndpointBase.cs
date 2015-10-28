using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace TypedRest
{
    /// <summary>
    /// Base class for building REST endpoints that represents a collection of <typeparamref name="TEntity"/>s as <typeparamref name="TElement"/>s.
    /// </summary>
    /// <typeparam name="TEntity">The type of entity the endpoint represents.</typeparam>
    /// <typeparam name="TElement">The specific type of <see cref="IElementEndpoint{TEntity}"/> to provide for individual <typeparamref name="TEntity"/>s.</typeparam>
    public abstract class CollectionEndpointBase<TEntity, TElement> : EndpointBase, ICollectionEndpoint<TEntity, TElement>
        where TElement : class, IElementEndpoint<TEntity>
    {
        /// <summary>
        /// Creates a new element collection endpoint.
        /// </summary>
        /// <param name="parent">The parent endpoint containing this one.</param>
        /// <param name="relativeUri">The URI of this endpoint relative to the <paramref name="parent"/>'s. Missing trailing slash will be appended automatically.</param>
        protected CollectionEndpointBase(IEndpoint parent, Uri relativeUri)
            : base(parent, relativeUri.EnsureTrailingSlash())
        {
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

        public TElement this[object id]
        {
            get { return GetElement(new Uri(id.ToString(), UriKind.Relative)); }
        }

        public virtual async Task<ICollection<TEntity>> ReadAllAsync(
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var response = await HttpClient.GetAsync(Uri, cancellationToken);
            await HandleErrorsAsync(response);

            return await response.Content.ReadAsAsync<List<TEntity>>(cancellationToken);
        }

        public virtual async Task<TElement> CreateAsync(TEntity entity,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var response = await HttpClient.PostAsync(Uri, entity, Serializer, cancellationToken);
            await HandleErrorsAsync(response);

            return (response.StatusCode == HttpStatusCode.Created)
                ? GetElement(response.Headers.Location)
                : null;
        }

        /// <summary>
        /// Instantiates a <typeparamref name="TElement"/> for an element in this set.
        /// </summary>
        protected abstract TElement GetElement(Uri relativeUri);
    }
}