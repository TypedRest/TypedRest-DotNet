using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace TypedRest.Errors;

/// <summary>
/// Handles errors in HTTP responses by mapping status codes to common exception types.
/// </summary>
public class DefaultErrorHandler : IErrorHandler
{
    public async Task HandleAsync(HttpResponseMessage response)
    {
        if (response.IsSuccessStatusCode) return;
        string? body = await response.Content.ReadAsStringAsync().NoContext();

        string? message = string.IsNullOrEmpty(body) ? null : ExtractMessage(body, response.Content.Headers.ContentType);
        message ??= $"{response.RequestMessage?.RequestUri} responded with {(int)response.StatusCode} {response.ReasonPhrase}";

        var exception = MapException(response.StatusCode, message);
        exception.SetHttpResponseHeaders(response.Headers);
        // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
        if (body != null) exception.SetHttpResponseBody(body);
        throw exception;
    }

    /// <summary>
    /// Tries to extract an error message from the response body.
    /// </summary>
    /// <param name="body">The response body in string form.</param>
    /// <param name="contentType">The content type of the response body.</param>
    protected virtual string? ExtractMessage(string body, MediaTypeHeaderValue? contentType)
    {
        string? mediaType = contentType?.MediaType;
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

    /// <summary>
    /// Maps the HTTP status code to an exception.
    /// </summary>
    /// <param name="statusCode">The status code.</param>
    /// <param name="message">An error message to include in the exception.</param>
    protected virtual Exception MapException(HttpStatusCode statusCode, string message)
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
