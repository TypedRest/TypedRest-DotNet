using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Security.Authentication;
using System.Threading;
using System.Threading.Tasks;

namespace TypedRest.Endpoints.Generic
{
    /// <summary>
    /// Provides extension methods for <see cref="IIndexerEndpoint{TElementEndpoint}"/> and <see cref="ICollectionEndpoint{TEntity,TElementEndpoint}"/>.
    /// </summary>
    public static class CollectionEndpointExtensions
    {
        /// <summary>
        /// Determines whether the collection contains a specific element.
        /// </summary>
        /// <typeparam name="TElementEndpoint">The type of <see cref="IEndpoint"/> to provide for individual entity.</typeparam>
        /// <param name="endpoint">The collection endpoint containing the element.</param>
        /// <param name="id">The ID identifying the entity in the collection.</param>
        /// <param name="cancellationToken">Used to cancel the request.</param>
        /// <exception cref="AuthenticationException"><see cref="HttpStatusCode.Unauthorized"/></exception>
        /// <exception cref="UnauthorizedAccessException"><see cref="HttpStatusCode.Forbidden"/></exception>
        /// <exception cref="HttpRequestException">Other non-success status code.</exception>
        /// <remarks>This is a convenience method equivalent to combining <see cref="IIndexerEndpoint{TElementEndpoint}.this[string]"/> with <see cref="IElementEndpoint.ExistsAsync"/>.</remarks>
        public static Task<bool> ContainsAsync<TElementEndpoint>(this IIndexerEndpoint<TElementEndpoint> endpoint, string id, CancellationToken cancellationToken = default)
            where TElementEndpoint : IElementEndpoint
            => endpoint[id].ExistsAsync(cancellationToken);

        /// <summary>
        /// Determines whether the collection contains a specific element.
        /// </summary>
        /// <typeparam name="TEntity">The type of individual elements in the collection.</typeparam>
        /// <typeparam name="TElementEndpoint">The type of <see cref="IEndpoint"/> to provide for individual <typeparamref name="TEntity"/>s.</typeparam>
        /// <param name="endpoint">The collection endpoint containing the element.</param>
        /// <param name="element">The element to be checked.</param>
        /// <param name="cancellationToken">Used to cancel the request.</param>
        /// <exception cref="AuthenticationException"><see cref="HttpStatusCode.Unauthorized"/></exception>
        /// <exception cref="UnauthorizedAccessException"><see cref="HttpStatusCode.Forbidden"/></exception>
        /// <exception cref="HttpRequestException">Other non-success status code.</exception>
        /// <remarks>This is a convenience method equivalent to combining <see cref="ICollectionEndpoint{TEntity,TElementEndpoint}.this[TEntity]"/> with <see cref="IElementEndpoint.ExistsAsync"/>.</remarks>
        public static Task<bool> ContainsAsync<TEntity, TElementEndpoint>(this ICollectionEndpoint<TEntity, TElementEndpoint> endpoint, TEntity element, CancellationToken cancellationToken = default)
            where TEntity : class
            where TElementEndpoint : IElementEndpoint<TEntity>
            => endpoint[element].ExistsAsync(cancellationToken);

        /// <summary>
        /// Sets/replaces an existing element in the collection.
        /// </summary>
        /// <typeparam name="TEntity">The type of individual elements in the collection.</typeparam>
        /// <typeparam name="TElementEndpoint">The type of <see cref="IEndpoint"/> to provide for individual <typeparamref name="TEntity"/>s.</typeparam>
        /// <param name="endpoint">The collection endpoint containing the element.</param>
        /// <param name="element">The new state of the element.</param>
        /// <param name="cancellationToken">Used to cancel the request.</param>
        /// <returns>The <typeparamref name="TEntity"/> as returned by the server, possibly with additional fields set. <c>null</c> if the server does not respond with a result entity.</returns>
        /// <exception cref="InvalidDataException"><see cref="HttpStatusCode.BadRequest"/></exception>
        /// <exception cref="AuthenticationException"><see cref="HttpStatusCode.Unauthorized"/></exception>
        /// <exception cref="UnauthorizedAccessException"><see cref="HttpStatusCode.Forbidden"/></exception>
        /// <exception cref="KeyNotFoundException"><see cref="HttpStatusCode.NotFound"/> or <see cref="HttpStatusCode.Gone"/></exception>
        /// <exception cref="HttpRequestException">Other non-success status code.</exception>
        /// <remarks>This is a convenience method equivalent to combining <see cref="ICollectionEndpoint{TEntity,TElementEndpoint}.this[TEntity]"/> with <see cref="IElementEndpoint{TEntity}.SetAsync"/>.</remarks>
        public static Task<TEntity?> SetAsync<TEntity, TElementEndpoint>(this ICollectionEndpoint<TEntity, TElementEndpoint> endpoint, TEntity element, CancellationToken cancellationToken = default)
            where TEntity : class
            where TElementEndpoint : IElementEndpoint<TEntity>
            => endpoint[element].SetAsync(element, cancellationToken);

        /// <summary>
        /// Modifies an existing element in the collection by merging changes on the server-side.
        /// </summary>
        /// <typeparam name="TEntity">The type of individual elements in the collection.</typeparam>
        /// <typeparam name="TElementEndpoint">The type of <see cref="IEndpoint"/> to provide for individual <typeparamref name="TEntity"/>s.</typeparam>
        /// <param name="endpoint">The collection endpoint containing the element.</param>
        /// <param name="element">The <typeparamref name="TEntity"/> data to merge with the existing element.</param>
        /// <param name="cancellationToken">Used to cancel the request.</param>
        /// <returns>The <typeparamref name="TEntity"/> as returned by the server, possibly with additional fields set. <c>null</c> if the server does not respond with a result entity.</returns>
        /// <exception cref="InvalidDataException"><see cref="HttpStatusCode.BadRequest"/></exception>
        /// <exception cref="AuthenticationException"><see cref="HttpStatusCode.Unauthorized"/></exception>
        /// <exception cref="UnauthorizedAccessException"><see cref="HttpStatusCode.Forbidden"/></exception>
        /// <exception cref="KeyNotFoundException"><see cref="HttpStatusCode.NotFound"/> or <see cref="HttpStatusCode.Gone"/></exception>
        /// <exception cref="HttpRequestException">Other non-success status code.</exception>
        /// <remarks>This is a convenience method equivalent to combining <see cref="ICollectionEndpoint{TEntity,TElementEndpoint}.this[TEntity]"/> with <see cref="IElementEndpoint{TEntity}.SetAsync"/>.</remarks>
        public static Task<TEntity?> MergeAsync<TEntity, TElementEndpoint>(this ICollectionEndpoint<TEntity, TElementEndpoint> endpoint, TEntity element, CancellationToken cancellationToken = default)
            where TEntity : class
            where TElementEndpoint : IElementEndpoint<TEntity>
            => endpoint[element].MergeAsync(element, cancellationToken);

        /// <summary>
        /// Deletes an existing element from the collection.
        /// </summary>
        /// <typeparam name="TElementEndpoint">The type of <see cref="IEndpoint"/> to provide for individual entities.</typeparam>
        /// <param name="endpoint">The collection endpoint containing the element.</param>
        /// <param name="id">The ID identifying the entity in the collection.</param>
        /// <param name="cancellationToken">Used to cancel the request.</param>
        /// <exception cref="AuthenticationException"><see cref="HttpStatusCode.Unauthorized"/></exception>
        /// <exception cref="UnauthorizedAccessException"><see cref="HttpStatusCode.Forbidden"/></exception>
        /// <exception cref="KeyNotFoundException"><see cref="HttpStatusCode.NotFound"/> or <see cref="HttpStatusCode.Gone"/></exception>
        /// <exception cref="HttpRequestException">Other non-success status code.</exception>
        /// <remarks>This is a convenience method equivalent to combining <see cref="IIndexerEndpoint{TElementEndpoint}.this[string]"/> with <see cref="IElementEndpoint.DeleteAsync"/>.</remarks>
        public static Task DeleteAsync<TElementEndpoint>(this IIndexerEndpoint<TElementEndpoint> endpoint, string id, CancellationToken cancellationToken = default)
            where TElementEndpoint : IElementEndpoint
            => endpoint[id].DeleteAsync(cancellationToken);

        /// <summary>
        /// Deletes an existing element from the collection.
        /// </summary>
        /// <typeparam name="TEntity">The type of individual elements in the collection.</typeparam>
        /// <typeparam name="TElementEndpoint">The type of <see cref="IEndpoint"/> to provide for individual <typeparamref name="TEntity"/>s.</typeparam>
        /// <param name="endpoint">The collection endpoint containing the element.</param>
        /// <param name="element">The element to be deleted.</param>
        /// <param name="cancellationToken">Used to cancel the request.</param>
        /// <exception cref="AuthenticationException"><see cref="HttpStatusCode.Unauthorized"/></exception>
        /// <exception cref="UnauthorizedAccessException"><see cref="HttpStatusCode.Forbidden"/></exception>
        /// <exception cref="KeyNotFoundException"><see cref="HttpStatusCode.NotFound"/> or <see cref="HttpStatusCode.Gone"/></exception>
        /// <exception cref="HttpRequestException">Other non-success status code.</exception>
        /// <remarks>This is a convenience method equivalent to combining <see cref="ICollectionEndpoint{TEntity,TElementEndpoint}.this[TEntity]"/> with <see cref="IElementEndpoint.DeleteAsync"/>.</remarks>
        public static Task DeleteAsync<TEntity, TElementEndpoint>(this ICollectionEndpoint<TEntity, TElementEndpoint> endpoint, TEntity element, CancellationToken cancellationToken = default)
            where TEntity : class
            where TElementEndpoint : IElementEndpoint<TEntity>
            => endpoint[element].DeleteAsync(cancellationToken);
    }
}
