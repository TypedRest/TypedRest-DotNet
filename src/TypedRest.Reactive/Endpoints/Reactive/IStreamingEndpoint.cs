namespace TypedRest.Endpoints.Reactive;

/// <summary>
/// Endpoint for a stream of <typeparamref name="TEntity"/>s.
/// </summary>
/// <typeparam name="TEntity">The type of individual elements in the stream.</typeparam>
public interface IStreamingEndpoint<out TEntity> : IEndpoint
{
    /// <summary>
    /// Provides an observable stream of entities. The observable is cold; HTTP communication only starts on <see cref="IObservable{T}.Subscribe"/>.
    /// </summary>
    IObservable<TEntity> GetObservable();
}