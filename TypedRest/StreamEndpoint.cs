using System;
using System.Net.Http.Headers;
using System.Reactive.Linq;

namespace TypedRest
{
    /// <summary>
    /// REST endpoint that represents a stream of elements. Uses the HTTP Range header and long polling.
    /// </summary>
    /// <typeparam name="TElement">The type of elements the endpoint represents.</typeparam>
    public class StreamEndpoint<TElement> : PaginationEndpoint<TElement>, IStreamEndpoint<TElement>
    {
        /// <summary>
        /// Creates a new stream endpoint.
        /// </summary>
        /// <param name="parent">The parent endpoint containing this one.</param>
        /// <param name="relativeUri">The URI of this endpoint relative to the <paramref name="parent"/>'s.</param>
        public StreamEndpoint(IEndpoint parent, Uri relativeUri) : base(parent, relativeUri)
        {
        }

        /// <summary>
        /// Creates a new stream endpoint.
        /// </summary>
        /// <param name="parent">The parent endpoint containing this one.</param>
        /// <param name="relativeUri">The URI of this endpoint relative to the <paramref name="parent"/>'s.</param>
        public StreamEndpoint(IEndpoint parent, string relativeUri) : base(parent, relativeUri)
        {
        }

        public virtual IObservable<TElement> GetStream(long startIndex = 0)
        {
            return Observable.Create<TElement>(async (observer, cancellationToken) =>
            {
                long currentStartIndex = startIndex;
                while (!cancellationToken.IsCancellationRequested)
                {
                    PartialResponse<TElement> response;
                    try
                    {
                        var range = (currentStartIndex >= 0)
                            // Offset
                            ? new RangeItemHeaderValue(currentStartIndex, null)
                            // Tail
                            : new RangeItemHeaderValue(null, -currentStartIndex);
                        response = await ReadPartialAsync(range, cancellationToken);
                    }
                    catch (IndexOutOfRangeException)
                    {
                        // No new data available yet, keep polling
                        continue;
                    }
                    catch (Exception ex)
                    {
                        observer.OnError(ex);
                        return;
                    }

                    foreach (var element in response.Elements)
                        observer.OnNext(element);

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