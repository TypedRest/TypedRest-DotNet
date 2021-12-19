namespace TypedRest;

internal class ByteContentMatcher : IMockedRequestMatcher
{
    private readonly byte[] _data;
    private readonly string _mimeType;

    public ByteContentMatcher(byte[] data, string mimeType)
    {
        _data = data;
        _mimeType = mimeType;
    }

    public bool Matches(HttpRequestMessage message)
        => message.Content != null
        && message.Content.ReadAsByteArrayAsync().Result.SequenceEqual(_data)
        && message.Content.Headers.ContentType!.MediaType == _mimeType;
}