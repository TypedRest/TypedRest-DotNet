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
    /// REST endpoint that represents a set of <typeparamref name="TEntity"/>s as <typeparamref name="TElement"/>s.
    /// </summary>
    /// <typeparam name="TEntity">The type of entity the endpoint represents.</typeparam>
    /// <typeparam name="TElement">The specific type of <see cref="IRestElement{TEntity}"/>s to provide for individual <typeparamref name="TEntity"/>s.</typeparam>
    public interface IRestSet<TEntity, TElement> : IRestEndpoint
        where TElement : class, IRestElement<TEntity>
    {
        /// <summary>
        /// Returns a <typeparamref name="TElement"/> for a specific element of this set. Does not perform any network traffic yet.
        /// </summary>
        /// <param name="id">The ID used to identify the element within the set.</param>
        TElement this[object id] { get; }

        /// <summary>
        /// Returns all <typeparamref name="TEntity"/>s.
        /// </summary>
        /// <param name="cancellationToken">Used to cancel the request.</param>
        /// <exception cref="UnauthorizedAccessException"><see cref="HttpStatusCode.Unauthorized"/> or <see cref="HttpStatusCode.Forbidden"/></exception>
        /// <exception cref="KeyNotFoundException"><see cref="HttpStatusCode.NotFound"/> or <see cref="HttpStatusCode.Gone"/></exception>
        /// <exception cref="HttpRequestException">Other non-success status code.</exception>
        Task<IEnumerable<TEntity>> ReadAllAsync(
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Creates a new <typeparamref name="TEntity"/>.
        /// </summary>
        /// <param name="entity">The new <typeparamref name="TEntity"/>.</param>
        /// <param name="cancellationToken">Used to cancel the request.</param>
        /// <returns>The newly created <typeparamref name="TEntity"/>; may be <see langword="null"/> if the server deferred creating the resource.</returns>
        /// <exception cref="InvalidDataException"><see cref="HttpStatusCode.BadRequest"/></exception>
        /// <exception cref="UnauthorizedAccessException"><see cref="HttpStatusCode.Unauthorized"/> or <see cref="HttpStatusCode.Forbidden"/></exception>
        /// <exception cref="KeyNotFoundException"><see cref="HttpStatusCode.NotFound"/> or <see cref="HttpStatusCode.Gone"/></exception>
        /// <exception cref="InvalidOperationException"><see cref="HttpStatusCode.Conflict"/></exception>
        /// <exception cref="HttpRequestException">Other non-success status code.</exception>
        Task<TElement> CreateAsync(TEntity entity,
            CancellationToken cancellationToken = default(CancellationToken));
    }
}