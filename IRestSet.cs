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
    /// REST endpoint that represents a set of entities.
    /// </summary>
    /// <typeparam name="TEntity">The type of entity the endpoint represents.</typeparam>
    public interface IRestSet<TEntity> : IRestEndpoint
    {
        /// <summary>
        /// Returns a <see cref="RestItem{TEntity}"/> for a specific item of this set. Does not perform any network traffic yet.
        /// </summary>
        /// <param name="id">The ID used to identify the item within the set.</param>
        IRestItem<TEntity> this[object id] { get; }

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
        /// <param name="item">The new <typeparamref name="TEntity"/>.</param>
        /// <param name="cancellationToken">Used to cancel the request.</param>
        /// <returns>The newly created <see cref="RestItem{TEntity}"/>; may be <see langword="null"/> if the server deferred creating the resource.</returns>
        /// <exception cref="InvalidDataException"><see cref="HttpStatusCode.BadRequest"/></exception>
        /// <exception cref="UnauthorizedAccessException"><see cref="HttpStatusCode.Unauthorized"/> or <see cref="HttpStatusCode.Forbidden"/></exception>
        /// <exception cref="KeyNotFoundException"><see cref="HttpStatusCode.NotFound"/> or <see cref="HttpStatusCode.Gone"/></exception>
        /// <exception cref="InvalidOperationException"><see cref="HttpStatusCode.Conflict"/></exception>
        /// <exception cref="HttpRequestException">Other non-success status code.</exception>
        Task<IRestItem<TEntity>> CreateAsync(TEntity item,
            CancellationToken cancellationToken = default(CancellationToken));
    }
}