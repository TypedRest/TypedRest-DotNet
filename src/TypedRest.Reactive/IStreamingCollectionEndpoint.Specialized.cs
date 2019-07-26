namespace TypedRest
{
    /// <summary>
    /// Endpoint for a collection of <typeparamref name="TEntity"/>s observable as an append-only stream.
    /// </summary>
    /// <typeparam name="TEntity">The type of entity the endpoint represents.</typeparam>
    public interface IStreamingCollectionEndpoint<TEntity> : IStreamingCollectionEndpoint<TEntity, IElementEndpoint<TEntity>>, ICollectionEndpoint<TEntity>
    {}
}
