namespace TypedRest.Endpoints.Rpc;

/// <summary>
/// Base class for building RPC endpoints.
/// </summary>
/// <param name="referrer">The endpoint used to navigate to this one.</param>
/// <param name="relativeUri">The URI of this endpoint relative to the <paramref name="referrer"/>'s. Add a <c>./</c> prefix here to imply a trailing slash <paramref name="referrer"/>'s URI.</param>
public abstract class RpcEndpointBase(IEndpoint referrer, Uri relativeUri)
    : EndpointBase(referrer, relativeUri), IRpcEndpoint
{
    /// <summary>
    /// Creates a new RPC endpoint with a relative URI.
    /// </summary>
    protected RpcEndpointBase(IEndpoint referrer, string relativeUri)
        : this(referrer, new Uri(relativeUri, UriKind.Relative)) {}

    public Task ProbeAsync(CancellationToken cancellationToken = default)
        => FinalizeAsync(() => HttpClient.OptionsAsync(Uri, cancellationToken));

    public bool? InvokeAllowed => IsMethodAllowed(HttpMethod.Post);
}
