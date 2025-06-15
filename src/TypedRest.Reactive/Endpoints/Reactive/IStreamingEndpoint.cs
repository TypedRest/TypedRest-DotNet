namespace TypedRest.Endpoints.Reactive;

/// <summary>
/// Endpoint for a stream of <typeparamref name="TEntity"/>s.
/// </summary>
/// <typeparam name="TEntity">The type of individual elements in the stream.</typeparam>
public interface IStreamingEndpoint<out TEntity> : IEndpoint
{
    /// <summary>
    /// Provides an observable stream of entities.
    /// </summary>
    /// <returns>A cold observable. HTTP communication only starts on <see cref="IObservable{T}.Subscribe"/>.</returns>
    IObservable<TEntity> GetObservable();
}
