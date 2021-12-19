using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace TypedRest.Errors
{
    /// <summary>
    /// Handles errors in HTTP responses by mapping status codes to common exception types.
    /// </summary>
    public class DefaultErrorHandler : IErrorHandler
    {
        public async Task HandleAsync(HttpResponseMessage response)
        {
            if (response.IsSuccessStatusCode) return;

            string? body = await response.Content.ReadAsStringAsync().NoContext();
            string message = ExtractJsonMessage(response, body)
                          ?? $"{response.RequestMessage?.RequestUri} responded with {(int)response.StatusCode} {response.ReasonPhrase}";

            var exception = MapException(response.StatusCode, message);
            exception.SetHttpResponseHeaders(response.Headers);
            // ReSharper disable once ConditionIsAlwaysTrueOrFalse
            if (body != null) exception.SetHttpResponseBody(body);
            throw exception;
        }

        private static string? ExtractJsonMessage(HttpResponseMessage response, string? body)
        {
            if (string.IsNullOrEmpty(body)) return null;

            string? mediaType = response.Content.Headers.ContentType?.MediaType;
            if (mediaType != "application/json" && (mediaType == null || !mediaType.EndsWith("+json"))) return null;

            try
            {
                var token = JToken.Parse(body);
                if (token.Type == JTokenType.Object)
                {
                    var messageNode = token["message"] ?? token["details"];
                    if (messageNode != null) return messageNode.ToString();
                }
            }
            catch (JsonException)
            {}

            return null;
        }

        private static Exception MapException(HttpStatusCode statusCode, string message)
        {
            var innerException = new HttpRequestException(message);
            return statusCode switch
            {
                HttpStatusCode.BadRequest => new InvalidDataException(message, innerException),
                HttpStatusCode.Unauthorized => new AuthenticationException(message, innerException),
                HttpStatusCode.Forbidden => new UnauthorizedAccessException(message, innerException),
                HttpStatusCode.NotFound or HttpStatusCode.Gone => new KeyNotFoundException(message, innerException),
                HttpStatusCode.Conflict or HttpStatusCode.PreconditionFailed or HttpStatusCode.RequestedRangeNotSatisfiable => new InvalidOperationException(message, innerException),
                HttpStatusCode.RequestTimeout => new TimeoutException(message, innerException),
                _ => innerException
            };
        }
    }
}
