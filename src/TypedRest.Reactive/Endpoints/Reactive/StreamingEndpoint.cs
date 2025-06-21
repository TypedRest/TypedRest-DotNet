namespace TypedRest.Endpoints.Reactive;

/// <summary>
/// Endpoint for a stream of <typeparamref name="TEntity"/>s using a persistent HTTP connection.
/// </summary>
/// <param name="referrer">The endpoint used to navigate to this one.</param>
/// <param name="relativeUri">The URI of this endpoint relative to the <paramref name="referrer"/>'s.</param>
/// <param name="separator">The character sequence used to detect that a new element starts in an HTTP stream.</param>
/// <typeparam name="TEntity">The type of individual elements in the stream.</typeparam>
public class StreamingEndpoint<TEntity>(IEndpoint referrer, Uri relativeUri, string separator = "\n")
    : EndpointBase(referrer, relativeUri), IStreamingEndpoint<TEntity>
{
    /// <summary>
    /// Creates a new streaming endpoint.
    /// </summary>
    /// <param name="referrer">The endpoint used to navigate to this one.</param>
    /// <param name="relativeUri">The URI of this endpoint relative to the <paramref name="referrer"/>'s. Add a <c>./</c> prefix here to imply a trailing slash <paramref name="referrer"/>'s URI.</param>
    /// <param name="separator">The character sequence used to detect that a new element starts in an HTTP stream.</param>
    public StreamingEndpoint(IEndpoint referrer, string relativeUri, string separator = "\n")
        : this(referrer, new Uri(relativeUri, UriKind.Relative), separator) {}

    /// <summary>
    /// The size of the buffer used to collect data for deserialization in bytes.
    /// </summary>
    public int BufferSize { get; set; } = 64 * 1024;

    public virtual IObservable<TEntity> GetObservable()
        => Observable.Create<TEntity>(async (observer, cancellationToken) =>
        {
            using var activity = StartActivity()?.AddTag("http.method", HttpMethod.Get.Method);

            using var response = await HttpClient.GetAsync(Uri, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
            await ErrorHandler.HandleAsync(response);

            var entityStream = new HttpEntityStream<TEntity>(response.Content, Serializer, separator, BufferSize);

            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    observer.OnNext(await entityStream.GetNextAsync(cancellationToken));
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
            }
        });
}
