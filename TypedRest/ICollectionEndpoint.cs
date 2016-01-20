﻿using System;
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
    /// <typeparam name="TEntity">The type of entity the endpoint represents.</typeparam>
    /// <typeparam name="TElementEndpoint">The specific type of <see cref="IElementEndpoint{TEntity}"/> to provide for individual <typeparamref name="TEntity"/>s.</typeparam>
    public interface ICollectionEndpoint<TEntity, TElementEndpoint> : IEndpoint
        where TElementEndpoint : class, IElementEndpoint<TEntity>
    {
        /// <summary>
        /// Returns a <typeparamref name="TElementEndpoint"/> for a specific child element of this collection. Does not perform any network traffic yet.
        /// </summary>
        /// <param name="relativeUri">The URI of the child endpoint relative to the this endpoint.</param>
        TElementEndpoint this[Uri relativeUri] { get; }

        /// <summary>
        /// Returns a <typeparamref name="TElementEndpoint"/> for a specific child element of this collection. Does not perform any network traffic yet.
        /// </summary>
        /// <param name="relativeUri">The URI of the child endpoint relative to the this endpoint.</param>
        TElementEndpoint this[string relativeUri] { get; }

        /// <summary>
        /// Returns an <see cref="ElementEndpoint{TEntity}"/> for a specific child element of this collection. Does not perform any network traffic yet.
        /// </summary>
        /// <param name="entity">A previously fetched instance of the entity to retrieve a new state for.</param>
        TElementEndpoint this[TEntity entity] { get; }

        /// <summary>
        /// Returns all <typeparamref name="TEntity"/>s.
        /// </summary>
        /// <param name="cancellationToken">Used to cancel the request.</param>
        /// <exception cref="UnauthorizedAccessException"><see cref="HttpStatusCode.Unauthorized"/> or <see cref="HttpStatusCode.Forbidden"/></exception>
        /// <exception cref="KeyNotFoundException"><see cref="HttpStatusCode.NotFound"/> or <see cref="HttpStatusCode.Gone"/></exception>
        /// <exception cref="HttpRequestException">Other non-success status code.</exception>
        Task<ICollection<TEntity>> ReadAllAsync(
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Shows whether the server has indicated that <seealso cref="CreateAsync"/> is currently allowed.
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
}