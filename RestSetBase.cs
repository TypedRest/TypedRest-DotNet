using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace TypedRest
{
    /// <summary>
    /// Base class for building REST endpoints that represents a set of <typeparamref name="TEntity"/> providing <typeparamref name="TItem"/>s.
    /// </summary>
    /// <typeparam name="TEntity">The type of entity the endpoint represents.</typeparam>
    /// <typeparam name="TItem">The specific type of <see cref="IRestItem{TEntity}"/>s to provide for individual <typeparamref name="TEntity"/>s.</typeparam>
    public abstract class RestSetBase<TEntity, TItem> : RestEndpoint
        where TItem : class, IRestItem<TEntity>
    {
        /// <summary>
        /// Creates a new item set endpoint.
        /// </summary>
        /// <param name="parent">The parent endpoint containing this one.</param>
        /// <param name="relativeUri">The URI of this endpoint relative to the <paramref name="parent"/>'s. Missing trailing slash will be appended automatically.</param>
        protected RestSetBase(IRestEndpoint parent, Uri relativeUri)
            : base(parent, relativeUri.EnsureTrailingSlash())
        {
        }

        /// <summary>
        /// Creates a new item set endpoint.
        /// </summary>
        /// <param name="parent">The parent endpoint containing this one.</param>
        /// <param name="relativeUri">The URI of this endpoint relative to the <paramref name="parent"/>'s. Missing trailing slash will be appended automatically.</param>
        protected RestSetBase(IRestEndpoint parent, string relativeUri)
            : this(parent, new Uri(relativeUri, UriKind.Relative))
        {
        }

        /// <summary>
        /// Returns a <see cref="RestItem{TEntity}"/> for a specific item of this set. Does not perform any network traffic yet.
        /// </summary>
        /// <param name="id">The ID used to identify the item within the set.</param>
        public TItem this[object id] => GetItem(new Uri(id.ToString(), UriKind.Relative));

        /// <summary>
        /// Returns all <typeparamref name="TEntity"/>s.
        /// </summary>
        /// <param name="cancellationToken">Used to cancel the request.</param>
        /// <exception cref="UnauthorizedAccessException"><see cref="HttpStatusCode.Unauthorized"/> or <see cref="HttpStatusCode.Forbidden"/></exception>
        /// <exception cref="KeyNotFoundException"><see cref="HttpStatusCode.NotFound"/> or <see cref="HttpStatusCode.Gone"/></exception>
        /// <exception cref="HttpRequestException">Other non-success status code.</exception>
        public virtual async Task<IEnumerable<TEntity>> ReadAllAsync(
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var response = await HttpClient.GetAsync(Uri, cancellationToken);
            await HandleErrors(response);

            return await response.Content.ReadAsAsync<List<TEntity>>(cancellationToken);
        }

        /// <summary>
        /// Creates a new <typeparamref name="TEntity"/>.
        /// </summary>
        /// <param name="item">The new <typeparamref name="TEntity"/>.</param>
        /// <param name="cancellationToken">Used to cancel the request.</param>
        /// <returns>The newly created <see cref="RestItem{TEntity}"/>; may be <see langword="null"/> if the server deferred creating the resource.</returns>
        /// <exception cref="InvalidDataException"><see cref="HttpStatusCode.BadRequest"/></exception>
        /// <exception cref="UnauthorizedAccessException"><see cref="HttpStatusCode.Unauthorized"/> or <see cref="HttpStatusCode.Forbidden"/></exception>
        /// <exception cref="KeyNotFoundException"><see cref="HttpStatusCode.NotFound"/> or <see cref="HttpStatusCode.Gone"/></exception>
        /// <exception cref="InvalidOperationException"><see cref="HttpStatusCode.Conflict"/></exception>
        /// <exception cref="HttpRequestException">Other non-success status code.</exception>
        public virtual async Task<TItem> CreateAsync(TEntity item,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var response = await HttpClient.PostAsJsonAsync(Uri, item, cancellationToken);
            await HandleErrors(response);

            return (response.StatusCode == HttpStatusCode.Created)
                ? GetItem(response.Headers.Location)
                : null;
        }

        /// <summary>
        /// Instantiates a child <typeparamref name="TEntity"/> for an item in this set.
        /// </summary>
        protected abstract TItem GetItem(Uri relativeUri);
    }
}