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
    /// Provides extension methods for <see cref="IElementEndpoint{TEntity}"/>.
    /// </summary>
    public static class ElementEndpointExtensions
    {
        /// <summary>
        /// Reads the current state of the entity, applies a change to it and stores the result. Applies optimistic concurrency using automatic retries.
        /// </summary>
        /// <typeparam name="TEntity">The type of entity the endpoint represents.</typeparam>
        /// <param name="endpoint">The endpoint representing the entity.</param>
        /// <param name="updateAction">A callback that takes the current state of the entity and applies the desired modifications.</param>
        /// <param name="maxRetries">The maximum number of retries to perform for optimistic concurrency before giving up.</param>
        /// <param name="cancellationToken">Used to cancel the request.</param>
        /// <returns>The <typeparamref name="TEntity"/> as returned by the server, possibly with additional fields set. <c>null</c> if the server does not respond with a result entity.</returns>
        /// <exception cref="InvalidDataException"><see cref="HttpStatusCode.BadRequest"/></exception>
        /// <exception cref="AuthenticationException"><see cref="HttpStatusCode.Unauthorized"/></exception>
        /// <exception cref="UnauthorizedAccessException"><see cref="HttpStatusCode.Forbidden"/></exception>
        /// <exception cref="KeyNotFoundException"><see cref="HttpStatusCode.NotFound"/> or <see cref="HttpStatusCode.Gone"/></exception>
        /// <exception cref="InvalidOperationException">The number of retries performed for optimistic concurrency exceeded <paramref name="maxRetries"/>.</exception>
        /// <exception cref="HttpRequestException">Other non-success status code.</exception>
        public static async Task<TEntity?> UpdateAsync<TEntity>(this IElementEndpoint<TEntity> endpoint, Action<TEntity> updateAction, int maxRetries = 3, CancellationToken cancellationToken = default)
            where TEntity : class
        {
            int retryCounter = 0;
            while (true)
            {
                var entity = await endpoint.ReadAsync(cancellationToken);
                cancellationToken.ThrowIfCancellationRequested();

                updateAction(entity);
                cancellationToken.ThrowIfCancellationRequested();

                try
                {
                    return await endpoint.SetAsync(entity, cancellationToken);
                }
                catch (InvalidOperationException)
                {
                    if (retryCounter++ >= maxRetries) throw;
                    cancellationToken.ThrowIfCancellationRequested();
                }
            }
        }
    }
}
