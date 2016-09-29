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
    /// REST endpoint that represents a collection of <typeparamref name="TEntity"/>s as <typeparamref name="TElementEndpoint"/>s.
    /// </summary>
    /// <remarks>Use the more constrained <see cref="ICollectionEndpoint{TEntity}"/> when possible.</remarks>
    /// <typeparam name="TEntity">The type of entity the endpoint represents.</typeparam>
    /// <typeparam name="TElementEndpoint">The type of <see cref="IEndpoint"/> to provide for individual <typeparamref name="TEntity"/>s.</typeparam>
    public interface ICollectionEndpoint<TEntity, TElementEndpoint> : IEndpoint
        where TElementEndpoint : class, IEndpoint
    {
        /// <summary>
        /// Returns a <typeparamref name="TElementEndpoint"/> for a specific child element of this collection.
        /// </summary>
        /// <param name="id">The ID identifying the entity in the collection.</param>
        TElementEndpoint this[string id] { get; }

        /// <summary>
        /// Returns an <see cref="ElementEndpoint{TEntity}"/> for a specific child element of this collection.
        /// </summary>
        /// <param name="entity">An existing entity to extract the ID from.</param>
        TElementEndpoint this[TEntity entity] { get; }

        /// <summary>
        /// Returns all <typeparamref name="TEntity"/>s.
        /// </summary>
        /// <param name="cancellationToken">Used to cancel the request.</param>
        /// <exception cref="UnauthorizedAccessException"><see cref="HttpStatusCode.Unauthorized"/> or <see cref="HttpStatusCode.Forbidden"/></exception>
        /// <exception cref="KeyNotFoundException"><see cref="HttpStatusCode.NotFound"/> or <see cref="HttpStatusCode.Gone"/></exception>
        /// <exception cref="HttpRequestException">Other non-success status code.</exception>
        Task<List<TEntity>> ReadAllAsync(CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Shows whether the server has indicated that <seealso cref="CreateAsync(TEntity,CancellationToken)"/> is currently allowed.
        /// </summary>
        /// <remarks>Uses cached data from last response.</remarks>
        /// <returns>An indicator whether the verb is allowed. If no request has been sent yet or the server did not specify allowed verbs <c>null</c> is returned.</returns>
        bool? CreateAllowed { get; }

        /// <summary>
        /// Creates a new <typeparamref name="TEntity"/>.
        /// </summary>
        /// <param name="entity">The new <typeparamref name="TEntity"/>.</param>
        /// <param name="cancellationToken">Used to cancel the request.</param>
        /// <returns>The newly created <typeparamref name="TEntity"/>; may be <c>null</c> if the server deferred creating the resource.</returns>
        /// <exception cref="InvalidDataException"><see cref="HttpStatusCode.BadRequest"/></exception>
        /// <exception cref="UnauthorizedAccessException"><see cref="HttpStatusCode.Unauthorized"/> or <see cref="HttpStatusCode.Forbidden"/></exception>
        /// <exception cref="KeyNotFoundException"><see cref="HttpStatusCode.NotFound"/> or <see cref="HttpStatusCode.Gone"/></exception>
        /// <exception cref="InvalidOperationException"><see cref="HttpStatusCode.Conflict"/></exception>
        /// <exception cref="HttpRequestException">Other non-success status code.</exception>
        Task<TElementEndpoint> CreateAsync(TEntity entity,
            CancellationToken cancellationToken = default(CancellationToken));
    }

    /// <summary>
    /// REST endpoint that represents a collection of <typeparamref name="TEntity"/>s as <see cref="IElementEndpoint{TEntity}"/>s.
    /// </summary>
    /// <typeparam name="TEntity">The type of entity the endpoint represents.</typeparam>
    public interface ICollectionEndpoint<TEntity> : ICollectionEndpoint<TEntity, IElementEndpoint<TEntity>>
    {
    }
}