namespace TypedRest
{
    /// <summary>
    /// REST endpoint that addresses a set of <typeparamref name="TElementEndpoint"/>s via IDs.
    /// </summary>
    /// <remarks>Use the more constrained <see cref="ICollectionEndpoint{TEntity}"/> when possible.</remarks>
    /// <typeparam name="TElementEndpoint">The type of <see cref="IEndpoint"/> to provide for individual elements.</typeparam>
    public interface IIndexerEndpoint<out TElementEndpoint> : IEndpoint
        where TElementEndpoint : IEndpoint
    {
        /// <summary>
        /// Returns a <typeparamref name="TElementEndpoint"/> for a specific child element of this collection.
        /// </summary>
        /// <param name="id">The ID identifying the entity in the collection.</param>
        TElementEndpoint this[string id] { get; }
    }
}
