using TypedRest.Endpoints.Generic;

namespace TypedRest.Endpoints.Reactive
{
    /// <summary>
    /// Endpoint for a collection of <typeparamref name="TEntity"/>s observable as an append-only stream.
    /// </summary>
    /// <remarks>Use the more constrained <see cref="IStreamingCollectionEndpoint{TEntity}"/> when possible.</remarks>
    /// <typeparam name="TEntity">The type of entity the endpoint represents.</typeparam>
    /// <typeparam name="TElementEndpoint">The type of <see cref="IEndpoint"/> to provide for individual <typeparamref name="TEntity"/>s.</typeparam>
    public interface IStreamingCollectionEndpoint<TEntity, out TElementEndpoint> : ICollectionEndpoint<TEntity, TElementEndpoint>
        where TEntity : class
        where TElementEndpoint : IElementEndpoint<TEntity>
    {
        /// <summary>
        /// Provides an observable stream of elements. The observable is cold; HTTP communication only starts on <see cref="IObservable{T}.Subscribe"/>.
        /// </summary>
        /// <param name="startIndex">The index of the first element to return in the stream. Use negative values to start counting from the end of the stream.</param>
        IObservable<TEntity> GetObservable(long startIndex = 0);
    }
}
