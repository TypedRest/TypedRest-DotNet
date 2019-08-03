using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
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
            string message = $"{response.RequestMessage?.RequestUri} responded with {(int)response.StatusCode} {response.ReasonPhrase}";

            string body = null;
            if (response.Content != null)
            {
                body = await response.Content.ReadAsStringAsync().NoContext();

                if (response.Content.Headers.ContentType?.MediaType == "application/json")
                {
                    try
                    {
                        var token = JToken.Parse(body);
                        if (token.Type == JTokenType.Object)
                        {
                            var messageNode = JToken.Parse(body)["message"];
                            if (messageNode != null) message = messageNode.ToString();
                        }
                    }
                    catch (JsonException)
                    {}
                }
            }

            switch (response.StatusCode)
            {
                case HttpStatusCode.BadRequest:
                    throw new InvalidDataException(message, new HttpRequestException(body));
                case HttpStatusCode.Unauthorized:
                    throw new AuthenticationException(message, new HttpRequestException(body));
                case HttpStatusCode.Forbidden:
                    throw new UnauthorizedAccessException(message, new HttpRequestException(body));
                case HttpStatusCode.NotFound:
                case HttpStatusCode.Gone:
                    throw new KeyNotFoundException(message, new HttpRequestException(body));
                case HttpStatusCode.Conflict:
                    throw new InvalidOperationException(message, new HttpRequestException(body));
                case HttpStatusCode.PreconditionFailed:
                    //throw new VersionNotFoundException(message, new HttpRequestException(body));
                    throw new InvalidOperationException(message, new HttpRequestException(body));
                case HttpStatusCode.RequestedRangeNotSatisfiable:
                    //throw new IndexOutOfRangeException(message, new HttpRequestException(body));
                    throw new InvalidOperationException(message, new HttpRequestException(body));
                case HttpStatusCode.RequestTimeout:
                    throw new TimeoutException(message, new HttpRequestException(body));
                default:
                    throw new HttpRequestException(message, new HttpRequestException(body));
            }
        }
    }
}
