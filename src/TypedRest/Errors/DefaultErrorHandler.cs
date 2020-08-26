using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Security.Authentication;
using System.Threading.Tasks;
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

            string message = $"{response.RequestMessage?.RequestUri} responded with {(int)response.StatusCode} {response.ReasonPhrase}";
            string? body = null;

            if (response.Content != null)
            {
                body = await response.Content.ReadAsStringAsync().NoContext();
                message = ExtractJsonMessage(response, body) ?? message;
            }

            var exception = MapException(response.StatusCode, message);
            exception.SetHttpResponseHeaders(response.Headers);
            if (body != null) exception.SetHttpResponseBody(body);
            throw exception;
        }

        private static string? ExtractJsonMessage(HttpResponseMessage response, string body)
        {
            string? mediaType = response.Content.Headers.ContentType?.MediaType;
            if (mediaType == "application/json" || (mediaType != null && mediaType.EndsWith("+json")))
            {
                try
                {
                    var token = JToken.Parse(body);
                    if (token.Type == JTokenType.Object)
                    {
                        var messageNode = JToken.Parse(body)["message"];
                        if (messageNode != null) return messageNode.ToString();
                    }
                }
                catch (JsonException)
                {}
            }

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
                HttpStatusCode.NotFound => new KeyNotFoundException(message, innerException),
                HttpStatusCode.Gone => new KeyNotFoundException(message, innerException),
                HttpStatusCode.Conflict => throw new InvalidOperationException(message, innerException),
                HttpStatusCode.PreconditionFailed => new InvalidOperationException(message, innerException),
                HttpStatusCode.RequestedRangeNotSatisfiable => new InvalidOperationException(message, innerException),
                HttpStatusCode.RequestTimeout => new TimeoutException(message, innerException),
                _ => innerException
            };
        }
    }
}
