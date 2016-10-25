using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Security.Authentication;
using System.Threading;
using System.Threading.Tasks;

namespace TypedRest
{
    /// <summary>
    /// Provides extension methods for <seealso cref="ICollectionEndpoint{TEntity,TElementEndpoint}"/>.
    /// </summary>
    public static class CollectionEndpointExtensions
    {
        /// <summary>
        /// Determines whether the collection contains a specific entity.
        /// </summary>
        /// <typeparam name="TEntity">The type of entity the endpoint represents.</typeparam>
        /// <typeparam name="TElementEndpoint">The type of <see cref="IEndpoint"/> to provide for individual <typeparamref name="TEntity"/>s.</typeparam>
        /// <param name="endpoint">The collection endpoint containing the element.</param>
        /// <param name="id">The ID identifying the entity in the collection.</param>
        /// <param name="cancellationToken">Used to cancel the request.</param>
        /// <exception cref="InvalidCredentialException"><see cref="HttpStatusCode.Unauthorized"/></exception>
        /// <exception cref="UnauthorizedAccessException"><see cref="HttpStatusCode.Forbidden"/></exception>
        /// <exception cref="HttpRequestException">Other non-success status code.</exception>
        /// <remarks>This is a convenience method equivalent to combining <seealso cref="ICollectionEndpoint{TEntity,TElementEndpoint}.this[string]"/> with <seealso cref="IElementEndpoint{TEntity}.ExistsAsync"/>.</remarks>
        public static Task<bool> ContainsAsync<TEntity, TElementEndpoint>(this ICollectionEndpoint<TEntity, TElementEndpoint> endpoint, string id, CancellationToken cancellationToken = default(CancellationToken))
            where TElementEndpoint : class, IElementEndpoint<TEntity>
        {
            return endpoint[id].ExistsAsync(cancellationToken);
        }

        /// <summary>
        /// Determines whether the collection contains a specific entity.
        /// </summary>
        /// <typeparam name="TEntity">The type of entity the endpoint represents.</typeparam>
        /// <typeparam name="TElementEndpoint">The type of <see cref="IEndpoint"/> to provide for individual <typeparamref name="TEntity"/>s.</typeparam>
        /// <param name="endpoint">The collection endpoint containing the element.</param>
        /// <param name="element">The element to be checked.</param>
        /// <param name="cancellationToken">Used to cancel the request.</param>
        /// <exception cref="InvalidCredentialException"><see cref="HttpStatusCode.Unauthorized"/></exception>
        /// <exception cref="UnauthorizedAccessException"><see cref="HttpStatusCode.Forbidden"/></exception>
        /// <exception cref="HttpRequestException">Other non-success status code.</exception>
        /// <remarks>This is a convenience method equivalent to combining <seealso cref="ICollectionEndpoint{TEntity,TElementEndpoint}.this[TEntity]"/> with <seealso cref="IElementEndpoint{TEntity}.ExistsAsync"/>.</remarks>
        public static Task<bool> ContainsAsync<TEntity, TElementEndpoint>(this ICollectionEndpoint<TEntity, TElementEndpoint> endpoint, TEntity element, CancellationToken cancellationToken = default(CancellationToken))
            where TElementEndpoint : class, IElementEndpoint<TEntity>
        {
            return endpoint[element].ExistsAsync(cancellationToken);
        }

        /// <summary>
        /// Updates an existing element in the collection.
        /// </summary>
        /// <typeparam name="TEntity">The type of entity the endpoint represents.</typeparam>
        /// <typeparam name="TElementEndpoint">The type of <see cref="IEndpoint"/> to provide for individual <typeparamref name="TEntity"/>s.</typeparam>
        /// <param name="endpoint">The collection endpoint containing the element.</param>
        /// <param name="element">The element to be updated.</param>
        /// <param name="cancellationToken">Used to cancel the request.</param>
        /// <returns>The <typeparamref name="TEntity"/> as returned by the server, possibly with additional fields set. <c>null</c> if the server does not respond with a result entity.</returns>
        /// <exception cref="InvalidDataException"><see cref="HttpStatusCode.BadRequest"/></exception>
        /// <exception cref="InvalidCredentialException"><see cref="HttpStatusCode.Unauthorized"/></exception>
        /// <exception cref="UnauthorizedAccessException"><see cref="HttpStatusCode.Forbidden"/></exception>
        /// <exception cref="KeyNotFoundException"><see cref="HttpStatusCode.NotFound"/> or <see cref="HttpStatusCode.Gone"/></exception>
        /// <exception cref="InvalidOperationException">The entity has changed since it was last retrieved with <see cref="IElementEndpoint{TEntity}.ReadAsync"/>. Your changes were rejected to prevent a lost update.</exception>
        /// <exception cref="HttpRequestException">Other non-success status code.</exception>
        /// <remarks>This is a convenience method equivalent to combining <seealso cref="ICollectionEndpoint{TEntity,TElementEndpoint}.this[TEntity]"/> with <seealso cref="IElementEndpoint{TEntity}.UpdateAsync"/>.</remarks>
        public static Task<TEntity> UpdateAsync<TEntity, TElementEndpoint>(this ICollectionEndpoint<TEntity, TElementEndpoint> endpoint, TEntity element, CancellationToken cancellationToken = default(CancellationToken))
            where TElementEndpoint : class, IElementEndpoint<TEntity>
        {
            return endpoint[element].UpdateAsync(element, cancellationToken);
        }

        /// <summary>
        /// Deletes an existing element from the collection.
        /// </summary>
        /// <typeparam name="TEntity">The type of entity the endpoint represents.</typeparam>
        /// <typeparam name="TElementEndpoint">The type of <see cref="IEndpoint"/> to provide for individual <typeparamref name="TEntity"/>s.</typeparam>
        /// <param name="endpoint">The collection endpoint containing the element.</param>
        /// <param name="id">The ID identifying the entity in the collection.</param>
        /// <param name="cancellationToken">Used to cancel the request.</param>
        /// <exception cref="InvalidDataException"><see cref="HttpStatusCode.BadRequest"/></exception>
        /// <exception cref="InvalidCredentialException"><see cref="HttpStatusCode.Unauthorized"/></exception>
        /// <exception cref="UnauthorizedAccessException"><see cref="HttpStatusCode.Forbidden"/></exception>
        /// <exception cref="KeyNotFoundException"><see cref="HttpStatusCode.NotFound"/> or <see cref="HttpStatusCode.Gone"/></exception>
        /// <exception cref="InvalidOperationException"><see cref="HttpStatusCode.Conflict"/></exception>
        /// <exception cref="HttpRequestException">Other non-success status code.</exception>
        /// <remarks>This is a convenience method equivalent to combining <seealso cref="ICollectionEndpoint{TEntity,TElementEndpoint}.this[string]"/> with <seealso cref="IElementEndpoint{TEntity}.DeleteAsync"/>.</remarks>
        public static Task DeleteAsync<TEntity, TElementEndpoint>(this ICollectionEndpoint<TEntity, TElementEndpoint> endpoint, string id, CancellationToken cancellationToken = default(CancellationToken))
            where TElementEndpoint : class, IElementEndpoint<TEntity>
        {
            return endpoint[id].DeleteAsync(cancellationToken);
        }

        /// <summary>
        /// Deletes an existing element from the collection.
        /// </summary>
        /// <typeparam name="TEntity">The type of entity the endpoint represents.</typeparam>
        /// <typeparam name="TElementEndpoint">The type of <see cref="IEndpoint"/> to provide for individual <typeparamref name="TEntity"/>s.</typeparam>
        /// <param name="endpoint">The collection endpoint containing the element.</param>
        /// <param name="element">The element to be deleted.</param>
        /// <param name="cancellationToken">Used to cancel the request.</param>
        /// <exception cref="InvalidDataException"><see cref="HttpStatusCode.BadRequest"/></exception>
        /// <exception cref="InvalidCredentialException"><see cref="HttpStatusCode.Unauthorized"/></exception>
        /// <exception cref="UnauthorizedAccessException"><see cref="HttpStatusCode.Forbidden"/></exception>
        /// <exception cref="KeyNotFoundException"><see cref="HttpStatusCode.NotFound"/> or <see cref="HttpStatusCode.Gone"/></exception>
        /// <exception cref="InvalidOperationException"><see cref="HttpStatusCode.Conflict"/></exception>
        /// <exception cref="HttpRequestException">Other non-success status code.</exception>
        /// <remarks>This is a convenience method equivalent to combining <seealso cref="ICollectionEndpoint{TEntity,TElementEndpoint}.this[TEntity]"/> with <seealso cref="IElementEndpoint{TEntity}.DeleteAsync"/>.</remarks>
        public static Task DeleteAsync<TEntity, TElementEndpoint>(this ICollectionEndpoint<TEntity, TElementEndpoint> endpoint, TEntity element, CancellationToken cancellationToken = default(CancellationToken))
            where TElementEndpoint : class, IElementEndpoint<TEntity>
        {
            return endpoint[element].DeleteAsync(cancellationToken);
        }
    }
}