namespace TypedRest;

/// <summary>
/// Provides extension methods for <see cref="Exception"/>.
/// </summary>
public static class ExceptionExtensions
{
    private const string HttpHeadersKey = "Http-Headers", HttpBodyKey = "Http-Body";

    extension(Exception exception)
    {
        /// <summary>
        /// Stores HTTP response headers in the exception.
        /// </summary>
        public void SetHttpResponseHeaders(HttpResponseHeaders headers)
        {
            try
            {
                exception.Data[HttpHeadersKey] = headers;
            }
            catch
            {
                // .NET Framework does not allow non-serializable data in exceptions
            }
        }

        /// <summary>
        /// Retrieves HTTP response headers from the exception.
        /// </summary>
        public HttpResponseHeaders? GetHttpResponseHeaders()
            => exception.Data[HttpHeadersKey] as HttpResponseHeaders;

        /// <summary>
        /// Stores an HTTP response body in the exception.
        /// </summary>
        public void SetHttpResponseBody(string body)
            => exception.Data[HttpBodyKey] = body;

        /// <summary>
        /// Retrieves an HTTP response body from the exception.
        /// </summary>
        public string? GetHttpResponseBody()
            => exception.Data[HttpBodyKey] as string;

        /// <summary>
        /// Waits for the duration specified in Retry-After header if it was set.
        /// </summary>
        public async Task HttpRetryDelayAsync(CancellationToken cancellationToken = default)
        {
            if (exception.GetHttpResponseHeaders()?.RetryAfter?.Delta is {} wait)
                await Task.Delay(wait, cancellationToken);
        }

        /// <summary>
        /// Concatenates the <see cref="Exception.Message"/>s of the entire <see cref="Exception.InnerException"/> tree.
        /// </summary>
        public string GetFullMessage()
        {
            var builder = new StringBuilder();
            for (Exception? current = exception; current != null; current = current.InnerException)
                builder.AppendLine(current.Message);
            return builder.ToString();
        }
    }
}
