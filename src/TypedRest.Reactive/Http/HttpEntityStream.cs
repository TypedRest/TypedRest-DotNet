namespace TypedRest.Http;

/// <summary>
/// Exposes an HTTP body as a stream of deserialized entities.
/// </summary>
/// <typeparam name="TEntity">The type of entity this stream provides.</typeparam>
public class HttpEntityStream<TEntity>
{
    private readonly HttpContent _content;
    private readonly MediaTypeFormatter _serializer;
    private readonly byte[] _separatorPattern;

    private Stream? _stream;
    private byte[] _buffer;
    private int _startIndex, _endIndex;

    /// <summary>
    /// Creates a new HTTP entity stream.
    /// </summary>
    /// <param name="content">The HTTP body.</param>
    /// <param name="serializer">Used to deserialize entities in the body.</param>
    /// <param name="separator">The character sequence used to detect that a new element starts in an HTTP stream.</param>
    /// <param name="bufferSize">The size of the buffer used to collect data for deserialization in bytes.</param>
    public HttpEntityStream(HttpContent content, MediaTypeFormatter serializer, string separator, int bufferSize)
    {
        _content = content ?? throw new ArgumentNullException(nameof(content));
        _serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
        _separatorPattern = Encoding.GetEncoding(content.Headers.ContentType?.CharSet ?? "UTF-8").GetBytes(separator ?? throw new ArgumentNullException(nameof(separator)));
        _buffer = new byte[bufferSize];
    }

    /// <summary>
    /// Retrieves the next entity from the stream.
    /// </summary>
    /// <param name="cancellationToken">Used to cancel the request.</param>
    /// <exception cref="EndOfStreamException">The stream has ended and there are no further entities.</exception>
    /// <exception cref="TaskCanceledException">The <paramref name="cancellationToken"/> was triggered.</exception>
    public async Task<TEntity> GetNextAsync(CancellationToken cancellationToken = default)
    {
        if (_stream == null)
            _stream = await _content.ReadAsStreamAsync();

        if (_startIndex >= _endIndex)
        {
            int count = await FillBufferAsync(cancellationToken);
            if (count == 0) throw new EndOfStreamException();
        }

        while (true)
        {
            cancellationToken.ThrowIfCancellationRequested();

            int separatorIndex = FindSeparator();
            if (separatorIndex != -1)
            {
                // Complete entity
                return await ParseAsync(separatorIndex, cancellationToken);
            }
            else if (separatorIndex < _endIndex)
            {
                // Potentially incomplete entity
                try
                {
                    return await ParseAsync(_endIndex, cancellationToken);
                }
                catch
                {
                    int count = await FillBufferAsync(cancellationToken);
                    if (count == 0) throw;
                }
            }
        }
    }

    private async Task<int> FillBufferAsync(CancellationToken cancellationToken)
    {
        if (_startIndex != 0) TrimBuffer();

        int count = await _stream!.ReadAsync(_buffer, _endIndex, _buffer.Length - _endIndex, cancellationToken);
        _endIndex += count;
        return count;
    }

    private void TrimBuffer()
    {
        if (_startIndex >= _endIndex)
        {
            _startIndex = 0;
            _endIndex = 0;
        }
        else
        {
            int newEndIndex = _endIndex - _startIndex;

            var newBuffer = new byte[_buffer.Length];
            Array.Copy(_buffer, _startIndex, newBuffer, 0, newEndIndex);

            _buffer = newBuffer;
            _startIndex = 0;
            _endIndex = newEndIndex;
        }
    }

    private int FindSeparator()
        => _buffer.IndexOfPattern(_separatorPattern, _startIndex, count: _endIndex - _startIndex);

    private async Task<TEntity> ParseAsync(int separatorIndex, CancellationToken cancellationToken)
    {
        using var subStream = new MemoryStream(_buffer, _startIndex, count: separatorIndex - _startIndex);
        var result = (TEntity)await _serializer.ReadFromStreamAsync(typeof(TEntity), subStream, _content, null, cancellationToken);
        _startIndex = separatorIndex + _separatorPattern.Length;
        return result;
    }
}