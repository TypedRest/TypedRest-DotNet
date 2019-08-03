namespace TypedRest.Endpoints.Generic
{
    /// <summary>
    /// Endpoint for a collection of <typeparamref name="TEntity"/>s addressable as <see cref="IElementEndpoint{TEntity}"/>s.
    /// </summary>
    /// <typeparam name="TEntity">The type of entity the endpoint represents.</typeparam>
    public interface ICollectionEndpoint<TEntity> : ICollectionEndpoint<TEntity, IElementEndpoint<TEntity>>
    {}
}
