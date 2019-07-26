using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using MorseCode.ITask;

namespace TypedRest
{
    /// <summary>
    /// RPC endpoint that returns <typeparamref name="TResult"/> as output when invoked.
    /// </summary>
    /// <typeparam name="TResult">The type of entity the endpoint returns as output.</typeparam>
    public interface IProducerEndpoint<out TResult> : IRpcEndpoint
    {
        /// <summary>
        /// Gets an entity from the producer.
        /// </summary>
        /// <param name="cancellationToken">Used to cancel the request.</param>
        /// <returns>The result returned by the server.</returns>
        /// <exception cref="AuthenticationException"><see cref="HttpStatusCode.Unauthorized"/></exception>
        /// <exception cref="UnauthorizedAccessException"><see cref="HttpStatusCode.Forbidden"/></exception>
        /// <exception cref="KeyNotFoundException"><see cref="HttpStatusCode.NotFound"/> or <see cref="HttpStatusCode.Gone"/></exception>
        /// <exception cref="InvalidOperationException"><see cref="HttpStatusCode.Conflict"/></exception>
        /// <exception cref="HttpRequestException">Other non-success status code.</exception>
        ITask<TResult> InvokeAsync(CancellationToken cancellationToken = default);
    }
}
