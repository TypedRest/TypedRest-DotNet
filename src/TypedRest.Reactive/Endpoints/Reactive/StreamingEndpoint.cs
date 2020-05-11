using System;
using System.IO;
using System.Net.Http;
using System.Reactive.Linq;
using TypedRest.Http;

namespace TypedRest.Endpoints.Reactive
{
    /// <summary>
    /// Endpoint for a stream of <typeparamref name="TEntity"/>s using a persistent HTTP connection.
    /// </summary>
    /// <typeparam name="TEntity">The type of individual elements in the stream.</typeparam>
    public class StreamingEndpoint<TEntity> : EndpointBase, IStreamingEndpoint<TEntity>
    {
        private readonly string _separator;

        /// <summary>
        /// Creates a new streaming endpoint.
        /// </summary>
        /// <param name="referrer">The endpoint used to navigate to this one.</param>
        /// <param name="relativeUri">The URI of this endpoint relative to the <paramref name="referrer"/>'s.</param>
        /// <param name="separator">The character sequence used to detect that a new element starts in an HTTP stream.</param>
        public StreamingEndpoint(IEndpoint referrer, Uri relativeUri, string separator = "\n")
            : base(referrer, relativeUri)
        {
            _separator = separator;
        }

        /// <summary>
        /// Creates a new streaming endpoint.
        /// </summary>
        /// <param name="referrer">The endpoint used to navigate to this one.</param>
        /// <param name="relativeUri">The URI of this endpoint relative to the <paramref name="referrer"/>'s. Prefix <c>./</c> to append a trailing slash to the <paramref name="referrer"/> URI if missing.</param>
        /// <param name="separator">The character sequence used to detect that a new element starts in an HTTP stream.</param>
        public StreamingEndpoint(IEndpoint referrer, string relativeUri, string separator = "\n")
            : base(referrer, relativeUri)
        {
            _separator = separator;
        }

        /// <summary>
        /// The size of the buffer used to collect data for deserialization in bytes.
        /// </summary>
        public int BufferSize { get; set; } = 64 * 1024;

        public virtual IObservable<TEntity> GetObservable()
            => Observable.Create<TEntity>(async (observer, cancellationToken) =>
            {
                using var response = await HttpClient.GetAsync(Uri, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
                if (!response.IsSuccessStatusCode)
                    await ErrorHandler.HandleAsync(response);

                var entityStream = new HttpEntityStream<TEntity>(response.Content, Serializer, _separator, BufferSize);

                while (!cancellationToken.IsCancellationRequested)
                {
                    TEntity entity;
                    try
                    {
                        entity = await entityStream.GetNextAsync(cancellationToken);
                    }
                    catch (OperationCanceledException)
                    {
                        return;
                    }
                    catch (EndOfStreamException)
                    {
                        observer.OnCompleted();
                        return;
                    }
                    catch (Exception ex)
                    {
                        observer.OnError(ex);
                        return;
                    }

                    observer.OnNext(entity);
                }
            });
    }
}
