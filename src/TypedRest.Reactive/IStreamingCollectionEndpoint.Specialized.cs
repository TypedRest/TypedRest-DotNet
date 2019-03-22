namespace TypedRest
{
    /// <summary>
    /// REST endpoint that represents a collection of <typeparamref name="TEntity"/>s as <see cref="IElementEndpoint{TEntity}"/>s that can also be streamed.
    /// </summary>
    /// <typeparam name="TEntity">The type of entity the endpoint represents.</typeparam>
    public interface IStreamingCollectionEndpoint<TEntity> : IStreamingCollectionEndpoint<TEntity, IElementEndpoint<TEntity>>, ICollectionEndpoint<TEntity>
    {}
}
