using System;

namespace TypedRest
{
    /// <summary>
    /// REST endpoint that represents a stream of <typeparamref name="TEntity"/>s.
    /// </summary>
    /// <typeparam name="TEntity">The type of entity the endpoint represents.</typeparam>
    /// <typeparam name="TElementEndpoint">The specific type of <see cref="IElementEndpoint{TEntity}"/> to provide for individual <typeparamref name="TEntity"/>s.</typeparam>
    public interface IStreamEndpoint<TEntity, TElementEndpoint> : IPagedCollectionEndpoint<TEntity, TElementEndpoint>
        where TElementEndpoint : class, IElementEndpoint<TEntity>
    {
        /// <summary>
        /// Provides an observable stream of elements.
        /// </summary>
        /// <param name="startIndex">The index of the first element to return in the stream. Use negative values to start counting from the end of the stream.</param>
        IObservable<TEntity> GetStream(long startIndex = 0);
    }
}