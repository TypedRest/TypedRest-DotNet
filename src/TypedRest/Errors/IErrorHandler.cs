using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Security.Authentication;
using System.Threading.Tasks;

namespace TypedRest.Errors
{
    /// <summary>
    /// Handles errors in HTTP responses.
    /// </summary>
    public interface IErrorHandler
    {
        /// <summary>
        /// Throws appropriate <see cref="Exception"/>s based on HTTP status codes and response bodies.
        /// </summary>
        /// <exception cref="InvalidDataException"><see cref="HttpStatusCode.BadRequest"/></exception>
        /// <exception cref="AuthenticationException"><see cref="HttpStatusCode.Unauthorized"/></exception>
        /// <exception cref="UnauthorizedAccessException"><see cref="HttpStatusCode.Forbidden"/></exception>
        /// <exception cref="KeyNotFoundException"><see cref="HttpStatusCode.NotFound"/> or <see cref="HttpStatusCode.Gone"/></exception>
        /// <exception cref="InvalidOperationException"><see cref="HttpStatusCode.Conflict"/>, <seealso cref="HttpStatusCode.PreconditionFailed"/> or <see cref="HttpStatusCode.RequestedRangeNotSatisfiable"/></exception>
        /// <exception cref="HttpRequestException">Other non-success status code.</exception>
        Task HandleAsync(HttpResponseMessage response);
    }
}
