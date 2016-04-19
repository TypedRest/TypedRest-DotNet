using System;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NLog;

namespace XProjectNamespaceX.WebService
{
    /// <summary>
    /// Logs HTTP requests to a log file.
    /// </summary>
    public class LoggingHandler : DelegatingHandler
    {
        private readonly ILogger _logger = LogManager.GetLogger("ApiRequest");

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request, CancellationToken cancellationToken)
        {
            // NOTE: Must read body before calling base method because stream becomes read-once
            string requestBody = (request.Content?.Headers.ContentType != null && request.Content.Headers.ContentType.MediaType.StartsWith("application/json"))
                ? await request.Content.ReadAsStringAsync()
                : null;

            var response = await base.SendAsync(request, cancellationToken);

            bool safeMethod = (request.Method == HttpMethod.Get || request.Method == HttpMethod.Head ||
                               request.Method == HttpMethod.Options || request.Method == HttpMethod.Trace);
            _logger.Log(
                level: response.IsSuccessStatusCode
                    ? (safeMethod ? LogLevel.Debug : LogLevel.Info)
                    : LogLevel.Warn,
                messageFunc: () =>
                {
                    var builder = new StringBuilder();
                    builder.AppendLine(request.Method + " " + request.RequestUri.PathAndQuery);

                    var context = request.GetRequestContext();
                    if (!string.IsNullOrEmpty(context.Principal.Identity.Name))
                        builder.AppendLine("User: " + context.Principal.Identity.Name);

                    if (!string.IsNullOrEmpty(request.Headers.From))
                        builder.AppendLine("From: " + request.Headers.From);

                    if (!string.IsNullOrEmpty(requestBody))
                        builder.AppendLine("Request body: " + requestBody);

                    builder.AppendLine("Response code: " + response.StatusCode);

                    var responseContent = response.Content as ObjectContent;
                    if (responseContent != null)
                        builder.AppendLine("Response content: " + responseContent.Value);

                    return builder.ToString().TrimEnd(Environment.NewLine.ToCharArray());
                });

            return response;
        }
    }
}
