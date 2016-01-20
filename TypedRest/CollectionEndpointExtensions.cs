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
    /// Provides extension methods for <seealso cref="ICollectionEndpoint{TEntity,TElementEndpoint}"/>.
    /// </summary>
    public static class CollectionEndpointExtensions
    {
        /// <summary>
        /// Updates an existing element in the collection.
        /// </summary>
        /// <typeparam name="TEntity">The type of entity the endpoint represents.</typeparam>
        /// <typeparam name="TElementEndpoint">The specific type of <see cref="IElementEndpoint{TEntity}"/> to provide for individual <typeparamref name="TEntity"/>s.</typeparam>
        /// <param name="endpoint">The collection endpoint containing the element.</param>
        /// <param name="element">The element to be updated.</param>
        /// <param name="cancellationToken">Used to cancel the request.</param>
        /// <exception cref="InvalidDataException"><see cref="HttpStatusCode.BadRequest"/></exception>
        /// <exception cref="UnauthorizedAccessException"><see cref="HttpStatusCode.Unauthorized"/> or <see cref="HttpStatusCode.Forbidden"/></exception>
        /// <exception cref="KeyNotFoundException"><see cref="HttpStatusCode.NotFound"/> or <see cref="HttpStatusCode.Gone"/></exception>
        /// <exception cref="InvalidOperationException"><see cref="HttpStatusCode.Conflict"/></exception>
        /// <exception cref="HttpRequestException">Other non-success status code.</exception>
        /// <remarks>This is a convenience method equivalent to combining <seealso cref="ICollectionEndpoint{TEntity,TElementEndpoint}.this[TEntity]"/> with <seealso cref="IElementEndpoint{TEntity}.UpdateAsync"/>.</remarks>
        public static async Task UpdateAsync<TEntity, TElementEndpoint>(this ICollectionEndpoint<TEntity, TElementEndpoint> endpoint, TEntity element, CancellationToken cancellationToken = default(CancellationToken))
            where TElementEndpoint : class, IElementEndpoint<TEntity>
        {
            await endpoint[element].UpdateAsync(element, cancellationToken);
        }

        /// <summary>
        /// Deletes an existing element from the collection.
        /// </summary>
        /// <typeparam name="TEntity">The type of entity the endpoint represents.</typeparam>
        /// <typeparam name="TElementEndpoint">The specific type of <see cref="IElementEndpoint{TEntity}"/> to provide for individual <typeparamref name="TEntity"/>s.</typeparam>
        /// <param name="endpoint">The collection endpoint containing the element.</param>
        /// <param name="element">The element to be deleted.</param>
        /// <param name="cancellationToken">Used to cancel the request.</param>
        /// <exception cref="InvalidDataException"><see cref="HttpStatusCode.BadRequest"/></exception>
        /// <exception cref="UnauthorizedAccessException"><see cref="HttpStatusCode.Unauthorized"/> or <see cref="HttpStatusCode.Forbidden"/></exception>
        /// <exception cref="KeyNotFoundException"><see cref="HttpStatusCode.NotFound"/> or <see cref="HttpStatusCode.Gone"/></exception>
        /// <exception cref="InvalidOperationException"><see cref="HttpStatusCode.Conflict"/></exception>
        /// <exception cref="HttpRequestException">Other non-success status code.</exception>
        /// <remarks>This is a convenience method equivalent to combining <seealso cref="ICollectionEndpoint{TEntity,TElementEndpoint}.this[TEntity]"/> with <seealso cref="IElementEndpoint{TEntity}.DeleteAsync"/>.</remarks>
        public static async Task DeleteAsync<TEntity, TElementEndpoint>(this ICollectionEndpoint<TEntity, TElementEndpoint> endpoint, TEntity element, CancellationToken cancellationToken = default(CancellationToken))
            where TElementEndpoint : class, IElementEndpoint<TEntity>
        {
            await endpoint[element].DeleteAsync(cancellationToken);
        }
    }
}