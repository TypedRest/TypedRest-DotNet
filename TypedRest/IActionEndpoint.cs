using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace TypedRest
{
    /// <summary>
    /// REST endpoint that represents an RPC-like action.
    /// </summary>
    public interface IActionEndpoint : ITriggerEndpoint
    {
        /// <summary>
        /// Triggers the action.
        /// </summary>
        /// <param name="cancellationToken">Used to cancel the request.</param>
        /// <exception cref="UnauthorizedAccessException"><see cref="HttpStatusCode.Unauthorized"/> or <see cref="HttpStatusCode.Forbidden"/></exception>
        /// <exception cref="KeyNotFoundException"><see cref="HttpStatusCode.NotFound"/> or <see cref="HttpStatusCode.Gone"/></exception>
        /// <exception cref="InvalidOperationException"><see cref="HttpStatusCode.Conflict"/></exception>
        /// <exception cref="HttpRequestException">Other non-success status code.</exception>
        Task TriggerAsync(CancellationToken cancellationToken = default(CancellationToken));
    }

    /// <summary>
    /// REST endpoint that represents an RPC-like action which takes <typeparamref name="TEntity"/> as input.
    /// </summary>
    /// <typeparam name="TEntity">The type of entity the endpoint takes as input.</typeparam>
    public interface IActionEndpoint<in TEntity> : ITriggerEndpoint
    {
        /// <summary>
        /// Triggers the action.
        /// </summary>
        /// <param name="entity">The <typeparamref name="TEntity"/> to post as input.</param>
        /// <param name="cancellationToken">Used to cancel the request.</param>
        /// <exception cref="UnauthorizedAccessException"><see cref="HttpStatusCode.Unauthorized"/> or <see cref="HttpStatusCode.Forbidden"/></exception>
        /// <exception cref="KeyNotFoundException"><see cref="HttpStatusCode.NotFound"/> or <see cref="HttpStatusCode.Gone"/></exception>
        /// <exception cref="InvalidOperationException"><see cref="HttpStatusCode.Conflict"/></exception>
        /// <exception cref="HttpRequestException">Other non-success status code.</exception>
        Task TriggerAsync(TEntity entity, CancellationToken cancellationToken = default(CancellationToken));
    }
}