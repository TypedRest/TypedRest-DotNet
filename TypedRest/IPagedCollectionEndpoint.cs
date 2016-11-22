using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Authentication;
using System.Threading;
using System.Threading.Tasks;

namespace TypedRest
{
    /// <summary>
    /// REST endpoint that represents a collection of <typeparamref name="TEntity"/>s as <typeparamref name="TElementEndpoint"/>s with pagination support.
    /// </summary>
    /// <remarks>Use the more constrained <see cref="IPagedCollectionEndpoint{TEntity}"/> when possible.</remarks>
    /// <typeparam name="TEntity">The type of entity the endpoint represents.</typeparam>
    /// <typeparam name="TElementEndpoint">The type of <see cref="IEndpoint"/> to provide for individual <typeparamref name="TEntity"/>s.</typeparam>
    public interface IPagedCollectionEndpoint<TEntity, TElementEndpoint> :
        ICollectionEndpoint<TEntity, TElementEndpoint>
        where TElementEndpoint : class, IEndpoint
    {
        /// <summary>
        /// Shows whether the server has indicated that <seealso cref="ReadRangeAsync"/> is allowed.
        /// </summary>
        /// <remarks>Uses cached data from last response.</remarks>
        /// <returns>An indicator whether the method is allowed. If no request has been sent yet.</returns>
        bool? ReadRangeAllowed { get; }

        /// <summary>
        /// Returns all <typeparamref name="TElementEndpoint"/>s within a specific range of the set.
        /// </summary>
        /// <param name="range">The range of elements to retrieve.</param>
        /// <param name="cancellationToken">Used to cancel the request.</param>
        /// <returns>A subset of the <typeparamref name="TElementEndpoint"/>s and the range they come from. May not exactly match the request <paramref name="range"/>.</returns>
        /// <exception cref="InvalidCredentialException"><see cref="HttpStatusCode.Unauthorized"/></exception>
        /// <exception cref="UnauthorizedAccessException"><see cref="HttpStatusCode.Forbidden"/></exception>
        /// <exception cref="KeyNotFoundException"><see cref="HttpStatusCode.NotFound"/> or <see cref="HttpStatusCode.Gone"/></exception>
        /// <exception cref="InvalidOperationException">The requested range is not satisfiable.</exception>
        /// <exception cref="HttpRequestException">Other non-success status code.</exception>
        Task<PartialResponse<TEntity>> ReadRangeAsync(RangeItemHeaderValue range,
            CancellationToken cancellationToken = default(CancellationToken));
    }

    /// <summary>
    /// REST endpoint that represents a collection of <typeparamref name="TEntity"/>s as <see cref="IElementEndpoint{TEntity}"/>s with pagination support.
    /// Use the more constrained <see cref="IPagedCollectionEndpoint{TEntity}"/> when possible.
    /// </summary>
    /// <typeparam name="TEntity">The type of entity the endpoint represents.</typeparam>
    public interface IPagedCollectionEndpoint<TEntity> : IPagedCollectionEndpoint<TEntity, IElementEndpoint<TEntity>>,
        ICollectionEndpoint<TEntity>
    {
    }
}