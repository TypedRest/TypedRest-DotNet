using System;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TypedRest
{
    /// <summary>
    /// Provides extension methods for <see cref="Exception"/>.
    /// </summary>
    public static class ExceptionExtensions
    {
        /// <summary>
        /// Concatenates the <see cref="Exception.Message"/>s of the entire <see cref="Exception.InnerException"/> tree.
        /// </summary>
        public static string GetFullMessage(this Exception exception)
        {
            var builder = new StringBuilder();
            for (Exception? current = exception; current != null; current = current.InnerException)
                builder.AppendLine(current.Message);
            return builder.ToString();
        }

        private const string HttpHeadersKey = "Http-Headers", HttpBodyKey = "Http-Body";

        /// <summary>
        /// Stores HTTP response headers in the exception.
        /// </summary>
        public static void SetHttpResponseHeaders(this Exception exception, HttpResponseHeaders headers)
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
        public static HttpResponseHeaders? GetHttpResponseHeaders(this Exception exception)
            => exception.Data[HttpHeadersKey] as HttpResponseHeaders;

        /// <summary>
        /// Stores an HTTP response body in the exception.
        /// </summary>
        public static void SetHttpResponseBody(this Exception exception, string body)
            => exception.Data[HttpBodyKey] = body;

        /// <summary>
        /// Retrieves an HTTP response body from the exception.
        /// </summary>
        public static string? GetHttpResponseBody(this Exception exception)
            => exception.Data[HttpBodyKey] as string;

        /// <summary>
        /// Waits for the duration specified in Retry-After header if it was set.
        /// </summary>
        public static async Task HttpRetryDelayAsync(this Exception exception, CancellationToken cancellationToken = default)
        {
            var wait = exception.GetHttpResponseHeaders()?.RetryAfter?.Delta;
            if (wait != null)
                await Task.Delay(wait.Value, cancellationToken);
        }
    }
}
