using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Security.Authentication;
using System.Threading;
using System.Threading.Tasks;

namespace TypedRest.Endpoints.Rpc
{
    /// <summary>
    /// RPC endpoint that takes <typeparamref name="TEntity"/> as input when invoked.
    /// </summary>
    /// <typeparam name="TEntity">The type of entity the endpoint takes as input.</typeparam>
    public interface IConsumerEndpoint<in TEntity> : IRpcEndpoint
    {
        /// <summary>
        /// Sends the entity to the consumer.
        /// </summary>
        /// <param name="entity">The <typeparamref name="TEntity"/> to post as input.</param>
        /// <param name="cancellationToken">Used to cancel the request.</param>
        /// <exception cref="AuthenticationException"><see cref="HttpStatusCode.Unauthorized"/></exception>
        /// <exception cref="UnauthorizedAccessException"><see cref="HttpStatusCode.Forbidden"/></exception>
        /// <exception cref="KeyNotFoundException"><see cref="HttpStatusCode.NotFound"/> or <see cref="HttpStatusCode.Gone"/></exception>
        /// <exception cref="InvalidOperationException"><see cref="HttpStatusCode.Conflict"/></exception>
        /// <exception cref="HttpRequestException">Other non-success status code.</exception>
        Task InvokeAsync(TEntity entity, CancellationToken cancellationToken = default);
    }
}
