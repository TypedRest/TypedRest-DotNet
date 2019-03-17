namespace TypedRest
{
    /// <summary>
    /// REST endpoint that represents a collection of <typeparamref name="TEntity"/>s as <see cref="IElementEndpoint{TEntity}"/>s.
    /// </summary>
    /// <typeparam name="TEntity">The type of entity the endpoint represents.</typeparam>
    public interface ICollectionEndpoint<TEntity> : ICollectionEndpoint<TEntity, IElementEndpoint<TEntity>>
    {}
}
