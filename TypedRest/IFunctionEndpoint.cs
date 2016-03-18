using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace TypedRest
{
    /// <summary>
    /// REST endpoint that represents a single RPC-like function which returns <typeparamref name="TResult"/>.
    /// </summary>
    /// <typeparam name="TResult">The type of entity the endpoint returns as a result.</typeparam>
    public interface IFunctionEndpoint<TResult> : ITriggerEndpoint
    {
        /// <summary>
        /// Triggers the function.
        /// </summary>
        /// <param name="cancellationToken">Used to cancel the request.</param>
        /// <exception cref="UnauthorizedAccessException"><see cref="HttpStatusCode.Unauthorized"/> or <see cref="HttpStatusCode.Forbidden"/></exception>
        /// <exception cref="KeyNotFoundException"><see cref="HttpStatusCode.NotFound"/> or <see cref="HttpStatusCode.Gone"/></exception>
        /// <exception cref="InvalidOperationException"><see cref="HttpStatusCode.Conflict"/></exception>
        /// <exception cref="HttpRequestException">Other non-success status code.</exception>
        Task<TResult> TriggerAsync(CancellationToken cancellationToken = default(CancellationToken));
    }

    /// <summary>
    /// REST endpoint that represents a single RPC-like function which returns <typeparamref name="TResult"/> and accepts <typeparamref name="TEntity"/> as input.
    /// </summary>
    /// <typeparam name="TEntity">The type of entity the endpoint takes as input.</typeparam>
    /// <typeparam name="TResult">The type of entity the endpoint returns as a result.</typeparam>
    public interface IFunctionEndpoint<in TEntity, TResult> : ITriggerEndpoint
    {
        /// <summary>
        /// Triggers the function.
        /// </summary>
        /// <param name="entity">The <typeparamref name="TEntity"/> to post.</param>
        /// <param name="cancellationToken">Used to cancel the request.</param>
        /// <exception cref="UnauthorizedAccessException"><see cref="HttpStatusCode.Unauthorized"/> or <see cref="HttpStatusCode.Forbidden"/></exception>
        /// <exception cref="KeyNotFoundException"><see cref="HttpStatusCode.NotFound"/> or <see cref="HttpStatusCode.Gone"/></exception>
        /// <exception cref="InvalidOperationException"><see cref="HttpStatusCode.Conflict"/></exception>
        /// <exception cref="HttpRequestException">Other non-success status code.</exception>
        Task<TResult> TriggerAsync(TEntity entity, CancellationToken cancellationToken = default(CancellationToken));
    }
}