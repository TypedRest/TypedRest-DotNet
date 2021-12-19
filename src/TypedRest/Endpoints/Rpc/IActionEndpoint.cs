namespace TypedRest.Endpoints.Rpc;

/// <summary>
/// RPC endpoint that is invoked with no input or output.
/// </summary>
public interface IActionEndpoint : IRpcEndpoint
{
    /// <summary>
    /// Invokes the action.
    /// </summary>
    /// <param name="cancellationToken">Used to cancel the request.</param>
    /// <exception cref="AuthenticationException"><see cref="HttpStatusCode.Unauthorized"/></exception>
    /// <exception cref="UnauthorizedAccessException"><see cref="HttpStatusCode.Forbidden"/></exception>
    /// <exception cref="KeyNotFoundException"><see cref="HttpStatusCode.NotFound"/> or <see cref="HttpStatusCode.Gone"/></exception>
    /// <exception cref="HttpRequestException">Other non-success status code.</exception>
    Task InvokeAsync(CancellationToken cancellationToken = default);
}