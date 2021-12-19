namespace TypedRest
{
    internal class MultipartFormContentMatcher : IMockedRequestMatcher
    {
        private readonly string _formField;
        private readonly byte[] _data;
        private readonly string _mimeType;
        private readonly string _fileName;

        public MultipartFormContentMatcher(string formField, byte[] data, string mimeType, string fileName)
        {
            _formField = formField;
            _data = data;
            _mimeType = mimeType;
            _fileName = fileName;
        }

        public bool Matches(HttpRequestMessage message)
        {
            var content = message.Content.ReadAsMultipartAsync().Result.Contents
                                 .Single(x => x.Headers.ContentDisposition?.Name == _formField);
            return content.ReadAsByteArrayAsync().Result.SequenceEqual(_data)
                && content.Headers.ContentType?.MediaType == _mimeType
                && content.Headers.ContentDisposition?.FileName == _fileName;
        }
    }
}
