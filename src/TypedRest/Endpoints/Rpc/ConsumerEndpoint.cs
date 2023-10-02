namespace TypedRest.Endpoints.Rpc;

/// <summary>
/// RPC endpoint that takes <typeparamref name="TEntity"/> as input when invoked.
/// </summary>
/// <typeparam name="TEntity">The type of entity the endpoint takes as input.</typeparam>
public class ConsumerEndpoint<TEntity> : RpcEndpointBase, IConsumerEndpoint<TEntity>
{
    /// <summary>
    /// Creates a new action endpoint with a relative URI.
    /// </summary>
    /// <param name="referrer">The endpoint used to navigate to this one.</param>
    /// <param name="relativeUri">The URI of this endpoint relative to the <paramref name="referrer"/>'s.</param>
    public ConsumerEndpoint(IEndpoint referrer, Uri relativeUri)
        : base(referrer, relativeUri)
    {}

    /// <summary>
    /// Creates a new action endpoint with a relative URI.
    /// </summary>
    /// <param name="referrer">The endpoint used to navigate to this one.</param>
    /// <param name="relativeUri">The URI of this endpoint relative to the <paramref name="referrer"/>'s. Add a <c>./</c> prefix here to imply a trailing slash <paramref name="referrer"/>'s URI.</param>
    public ConsumerEndpoint(IEndpoint referrer, string relativeUri)
        : base(referrer, relativeUri)
    {}

    public async Task InvokeAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        if (entity == null) throw new ArgumentNullException(nameof(entity));

        await FinalizeAsync(() => HttpClient.PostAsync(Uri, entity, Serializers[0], cancellationToken)).NoContext();
    }
}
