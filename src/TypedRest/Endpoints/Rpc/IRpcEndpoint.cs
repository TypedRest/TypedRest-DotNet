namespace TypedRest.Endpoints.Rpc;

/// <summary>
/// An endpoint for a non-RESTful resource that acts like a callable function.
/// </summary>
public interface IRpcEndpoint : IEndpoint
{
    /// <summary>
    /// Queries the server about capabilities of the endpoint without performing any action.
    /// </summary>
    /// <param name="cancellationToken">Used to cancel the request.</param>
    /// <exception cref="AuthenticationException"><see cref="HttpStatusCode.Unauthorized"/></exception>
    /// <exception cref="UnauthorizedAccessException"><see cref="HttpStatusCode.Forbidden"/></exception>
    /// <exception cref="KeyNotFoundException"><see cref="HttpStatusCode.NotFound"/> or <see cref="HttpStatusCode.Gone"/></exception>
    /// <exception cref="HttpRequestException">Other non-success status code.</exception>
    Task ProbeAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Indicates whether the server has specified the invoke method is currently allowed.
    /// </summary>
    /// <remarks>Uses cached data from last response.</remarks>
    /// <returns><c>true</c> if the method is allowed, <c>false</c> if the method is not allowed, <c>null</c> If no request has been sent yet or the server did not specify allowed methods.</returns>
    bool? InvokeAllowed { get; }
}
