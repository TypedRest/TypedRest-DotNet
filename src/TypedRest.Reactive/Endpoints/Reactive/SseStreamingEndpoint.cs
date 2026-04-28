using System.Net.ServerSentEvents;

namespace TypedRest.Endpoints.Reactive;

/// <summary>
/// Endpoint for a stream of <typeparamref name="TEntity"/>s using Server-Sent Events (SSE).
/// </summary>
/// <remarks>
/// Sends <c>Accept: text/event-stream</c>. By default, transparently reconnects on connection drops or transient errors,
/// honoring the server-supplied <c>retry:</c> interval and resuming via the <c>Last-Event-ID</c> header.
/// </remarks>
/// <param name="referrer">The endpoint used to navigate to this one.</param>
/// <param name="relativeUri">The URI of this endpoint relative to the <paramref name="referrer"/>'s.</param>
/// <param name="eventType">If set, only events with this <c>event:</c> type are emitted; others are ignored. <c>null</c> emits all events.</param>
/// <typeparam name="TEntity">The type of individual elements in the stream.</typeparam>
public class SseStreamingEndpoint<TEntity>(IEndpoint referrer, Uri relativeUri, string? eventType = null)
    : EndpointBase(referrer, relativeUri), IStreamingEndpoint<TEntity>
{
    /// <summary>
    /// Creates a new SSE streaming endpoint.
    /// </summary>
    /// <param name="referrer">The endpoint used to navigate to this one.</param>
    /// <param name="relativeUri">The URI of this endpoint relative to the <paramref name="referrer"/>'s. Add a <c>./</c> prefix here to imply a trailing slash <paramref name="referrer"/>'s URI.</param>
    /// <param name="eventType">If set, only events with this <c>event:</c> type are emitted; others are ignored. <c>null</c> emits all events.</param>
    public SseStreamingEndpoint(IEndpoint referrer, string relativeUri, string? eventType = null)
        : this(referrer, new Uri(relativeUri, UriKind.Relative), eventType) {}

    /// <summary>
    /// Whether to transparently reconnect on connection drops, transient transport errors, and 5xx responses.
    /// </summary>
    public bool AutoReconnect { get; set; } = true;

    /// <summary>
    /// The reconnection interval used when the server has not (yet) supplied one via the SSE <c>retry:</c> field.
    /// </summary>
    public TimeSpan DefaultReconnectionInterval { get; set; } = TimeSpan.FromSeconds(3);

    public virtual IObservable<TEntity> GetObservable()
        => Observable.Create<TEntity>(async (observer, cancellationToken) =>
        {
            string? lastEventId = null;
            var reconnectionInterval = DefaultReconnectionInterval;

            while (!cancellationToken.IsCancellationRequested)
            {
                bool reconnect;
                try
                {
                    reconnect = await ConsumeOnceAsync(observer, lastId => lastEventId = lastId ?? lastEventId, interval => reconnectionInterval = interval, lastEventId, cancellationToken);
                }
                catch (OperationCanceledException)
                {
                    return;
                }
                catch (Exception ex)
                {
                    if (AutoReconnect && IsTransient(ex)) reconnect = true;
                    else { observer.OnError(ex); return; }
                }

                if (!reconnect)
                {
                    observer.OnCompleted();
                    return;
                }

                try { await Task.Delay(reconnectionInterval, cancellationToken); }
                catch (OperationCanceledException) { return; }
            }
        });

    private async Task<bool> ConsumeOnceAsync(IObserver<TEntity> observer, Action<string?> updateLastEventId, Action<TimeSpan> updateReconnectionInterval, string? lastEventId, CancellationToken cancellationToken)
    {
        using var activity = StartActivity()?.AddTag("http.method", HttpMethod.Get.Method);

        using var request = new HttpRequestMessage(HttpMethod.Get, Uri);
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("text/event-stream"));
        if (!string.IsNullOrEmpty(lastEventId))
            request.Headers.TryAddWithoutValidation("Last-Event-ID", lastEventId);

        using var response = await HttpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken);

        if (response.StatusCode == HttpStatusCode.NoContent)
            return false;

        if (AutoReconnect && (int)response.StatusCode >= 500)
            return true;

        await ErrorHandler.HandleAsync(response);

#if NET
        var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
#else
        var stream = await response.Content.ReadAsStreamAsync();
#endif
        var parser = SseParser.Create(stream);

        try
        {
            await foreach (var item in parser.EnumerateAsync(cancellationToken))
            {
                if (eventType != null && item.EventType != eventType) continue;
                observer.OnNext(await DeserializeAsync(item.Data, response.Content, cancellationToken));
            }
        }
        finally
        {
            updateLastEventId(parser.LastEventId);
            if (parser.ReconnectionInterval != Timeout.InfiniteTimeSpan && parser.ReconnectionInterval >= TimeSpan.Zero)
                updateReconnectionInterval(parser.ReconnectionInterval);
        }

        return AutoReconnect;
    }

    private async Task<TEntity> DeserializeAsync(string data, HttpContent content, CancellationToken cancellationToken)
    {
        var bytes = Encoding.UTF8.GetBytes(data);
        using var stream = new MemoryStream(bytes);
        return (TEntity)await Serializer.ReadFromStreamAsync(typeof(TEntity), stream, content, null, cancellationToken);
    }

    private static bool IsTransient(Exception ex)
        => ex is HttpRequestException or IOException;
}
