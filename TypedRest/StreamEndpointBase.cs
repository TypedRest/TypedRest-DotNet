using System;
using System.Net.Http.Headers;
using System.Reactive.Linq;

namespace TypedRest
{
    /// <summary>
    /// Base class for building REST endpoints that represents a stream of <typeparamref name="TEntity"/>s as <typeparamref name="TElementEndpoint"/>s. Uses the HTTP Range header.
    /// </summary>
    /// <typeparam name="TEntity">The type of entity the endpoint represents.</typeparam>
    /// <typeparam name="TElementEndpoint">The specific type of <see cref="IElementEndpoint{TEntity}"/> to provide for individual <typeparamref name="TEntity"/>s.</typeparam>
    public abstract class StreamEndpointBase<TEntity, TElementEndpoint> : PagedCollectionEndpointBase<TEntity, TElementEndpoint>, IStreamEndpoint<TEntity, TElementEndpoint>
        where TElementEndpoint : class, IElementEndpoint<TEntity>
    {
        /// <summary>
        /// Creates a new stream endpoint.
        /// </summary>
        /// <param name="parent">The parent endpoint containing this one.</param>
        /// <param name="relativeUri">The URI of this endpoint relative to the <paramref name="parent"/>'s. Missing trailing slash will be appended automatically.</param>
        protected StreamEndpointBase(IEndpoint parent, Uri relativeUri)
            : base(parent, relativeUri)
        {
        }

        /// <summary>
        /// Creates a new stream endpoint.
        /// </summary>
        /// <param name="parent">The parent endpoint containing this one.</param>
        /// <param name="relativeUri">The URI of this endpoint relative to the <paramref name="parent"/>'s. Missing trailing slash will be appended automatically.</param>
        protected StreamEndpointBase(IEndpoint parent, string relativeUri)
            : base(parent, relativeUri)
        {
        }

        public virtual IObservable<TEntity> GetStream(long startIndex = 0)
        {
            return Observable.Create<TEntity>(async (observer, cancellationToken) =>
            {
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
                    catch (Exception ex)
                    {
                        observer.OnError(ex);
                        return;
                    }

                    foreach (var entity in response.Elements)
                        observer.OnNext(entity);

                    if (response.Range == null || response.EndReached)
                    {
                        observer.OnCompleted();
                        return;
                    }

                    // Continue polling for more data
                    currentStartIndex = response.Range.To.Value + 1;
                }
            });
        }
    }
}