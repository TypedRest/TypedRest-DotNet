using System;

namespace TypedRest
{
    /// <summary>
    /// REST endpoint that represents a collection of <typeparamref name="TEntity"/>s as <typeparamref name="TElementEndpoint"/>s that can also be streamed.
    /// </summary>
    /// <remarks>Use the more constrained <see cref="IStreamingCollectionEndpoint{TEntity}"/> when possible.</remarks>
    /// <typeparam name="TEntity">The type of entity the endpoint represents.</typeparam>
    /// <typeparam name="TElementEndpoint">The type of <see cref="IEndpoint"/> to provide for individual <typeparamref name="TEntity"/>s.</typeparam>
    public interface IStreamingCollectionEndpoint<TEntity, out TElementEndpoint> : ICollectionEndpoint<TEntity, TElementEndpoint>
        where TElementEndpoint : class, IEndpoint
    {
        /// <summary>
        /// Provides an observable stream of elements. The observable is cold; HTTP communication only starts on <see cref="IObservable{T}.Subscribe"/>.
        /// </summary>
        /// <param name="startIndex">The index of the first element to return in the stream. Use negative values to start counting from the end of the stream.</param>
        IObservable<TEntity> GetObservable(long startIndex = 0);
    }
}
