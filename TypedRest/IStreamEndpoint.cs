using System;

namespace TypedRest
{
    /// <summary>
    /// REST endpoint that represents a stream of <typeparamref name="TEntity"/>s as <typeparamref name="TElementEndpoint"/>s.
    /// </summary>
    /// <remarks>Use the more constrained <see cref="IStreamEndpoint{TEntity}"/> when possible.</remarks>
    /// <typeparam name="TEntity">The type of entity the endpoint represents.</typeparam>
    /// <typeparam name="TElementEndpoint">The type of <see cref="IEndpoint"/> to provide for individual <typeparamref name="TEntity"/>s.</typeparam>
    public interface IStreamEndpoint<TEntity, TElementEndpoint> : ICollectionEndpoint<TEntity, TElementEndpoint>
        where TElementEndpoint : class, IEndpoint
    {
        /// <summary>
        /// Provides an observable stream of elements.
        /// </summary>
        /// <param name="startIndex">The index of the first element to return in the stream. Use negative values to start counting from the end of the stream.</param>
        IObservable<TEntity> GetStream(long startIndex = 0);
    }

    /// <summary>
    /// REST endpoint that represents a stream of <typeparamref name="TEntity"/>s as <see cref="IElementEndpoint{TEntity}"/>s.
    /// </summary>
    /// <typeparam name="TEntity">The type of entity the endpoint represents.</typeparam>
    public interface IStreamEndpoint<TEntity> : IStreamEndpoint<TEntity, IElementEndpoint<TEntity>>,
        ICollectionEndpoint<TEntity>
    {
    }
}