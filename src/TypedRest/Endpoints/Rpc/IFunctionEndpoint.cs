namespace TypedRest.Endpoints.Rpc;

/// <summary>
/// RPC endpoint that takes <typeparamref name="TEntity"/> as input and returns <typeparamref name="TResult"/> as output when invoked.
/// </summary>
/// <typeparam name="TEntity">The type of entity the endpoint takes as input.</typeparam>
/// <typeparam name="TResult">The type of entity the endpoint returns as output.</typeparam>
public interface IFunctionEndpoint<in TEntity, TResult> : IRpcEndpoint
{
    /// <summary>
    /// Invokes the function.
    /// </summary>
    /// <param name="entity">The entity to post as input.</param>
    /// <param name="cancellationToken">Used to cancel the request.</param>
    /// <returns>The result returned by the server.</returns>
    /// <exception cref="AuthenticationException"><see cref="HttpStatusCode.Unauthorized"/></exception>
    /// <exception cref="UnauthorizedAccessException"><see cref="HttpStatusCode.Forbidden"/></exception>
    /// <exception cref="KeyNotFoundException"><see cref="HttpStatusCode.NotFound"/> or <see cref="HttpStatusCode.Gone"/></exception>
    /// <exception cref="HttpRequestException">Other non-success status code.</exception>
    Task<TResult> InvokeAsync(TEntity entity, CancellationToken cancellationToken = default);
}