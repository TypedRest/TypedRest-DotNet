using TypedRest.Endpoints.Generic;

namespace TypedRest.Endpoints.Reactive;

/// <summary>
/// Endpoint for a collection of <typeparamref name="TEntity"/>s observable as an append-only stream using long-polling.
/// </summary>
/// <typeparam name="TEntity">The type of individual elements in the collection.</typeparam>
/// <typeparam name="TElementEndpoint">The type of <see cref="IEndpoint"/> to provide for individual <typeparamref name="TEntity"/>s. Must have a public constructor with an <see cref="IEndpoint"/> and an <see cref="Uri"/> or string parameter.</typeparam>
public class StreamingCollectionEndpoint<TEntity, TElementEndpoint> : CollectionEndpoint<TEntity, TElementEndpoint>, IStreamingCollectionEndpoint<TEntity, TElementEndpoint>
    where TEntity : class
    where TElementEndpoint : class, IElementEndpoint<TEntity>
{
    /// <summary>
    /// Creates a new streaming collection endpoint.
    /// </summary>
    /// <param name="referrer">The endpoint used to navigate to this one.</param>
    /// <param name="relativeUri">The URI of this endpoint relative to the <paramref name="referrer"/>'s.</param>
    public StreamingCollectionEndpoint(IEndpoint referrer, Uri relativeUri)
        : base(referrer, relativeUri)
    {}

    /// <summary>
    /// Creates a new streaming collection endpoint.
    /// </summary>
    /// <param name="referrer">The endpoint used to navigate to this one.</param>
    /// <param name="relativeUri">The URI of this endpoint relative to the <paramref name="referrer"/>'s. Add a <c>./</c> prefix here to imply a trailing slash <paramref name="referrer"/>'s URI.</param>
    public StreamingCollectionEndpoint(IEndpoint referrer, string relativeUri)
        : base(referrer, relativeUri)
    {}

    public IObservable<TEntity> GetObservable(long startIndex = 0)
        => Observable.Create<TEntity>(async (observer, cancellationToken) =>
        {
            using var activity = StartActivity();

            long currentStartIndex = startIndex;
            while (!cancellationToken.IsCancellationRequested)
            {
                PartialResponse<TEntity> response;
                try
                {
                    var range = (currentStartIndex >= 0)
                        // Offset
                        ? new RangeItemHeaderValue(currentStartIndex, null)
                        // Tail
                        : new RangeItemHeaderValue(null, -currentStartIndex);
                    response = await ReadRangeAsync(range, cancellationToken);
                }
                catch (InvalidOperationException)
                {
                    // No new data available yet, keep polling
                    continue;
                }

                foreach (var entity in response.Elements)
                    observer.OnNext(entity);

                if (response.EndReached)
                {
                    observer.OnCompleted();
                    return;
                }

                // Continue polling for more data
                if (response.Range?.To is {} value) currentStartIndex = value + 1;
                else return;
            }
        });
}
