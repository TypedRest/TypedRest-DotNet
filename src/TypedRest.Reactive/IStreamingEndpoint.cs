using System;

namespace TypedRest
{
    /// <summary>
    /// REST endpoint that represents a stream of <typeparamref name="TEntity"/>s.
    /// </summary>
    /// <typeparam name="TEntity">The type of entity the endpoint represents.</typeparam>
    public interface IStreamingEndpoint<out TEntity> : IEndpoint
    {
        /// <summary>
        /// Provides an observable stream of elements. The observable is cold; HTTP communication only starts on <see cref="IObservable{T}.Subscribe"/>.
        /// </summary>
        IObservable<TEntity> GetObservable();
    }
}
