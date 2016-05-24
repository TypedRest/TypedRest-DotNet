using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Web.Http.Filters;

namespace XProjectNamespaceX.WebService
{
    /// <summary>
    /// Maps different <see cref="Exception"/> types to appropriate <seealso cref="HttpStatusCode"/>s.
    /// </summary>
    public class ExceptionHandlingAttribute : ExceptionFilterAttribute
    {
        public override void OnException(HttpActionExecutedContext context)
        {
            if (context.Exception is InvalidDataException || context.Exception is FormatException)
                context.Response = context.Request.CreateErrorResponse(HttpStatusCode.BadRequest, context.Exception.Message);
            if (context.Exception is UnauthorizedAccessException)
                context.Response = context.Request.CreateErrorResponse(HttpStatusCode.Forbidden, context.Exception.Message);
            else if (context.Exception is KeyNotFoundException)
                context.Response = context.Request.CreateErrorResponse(HttpStatusCode.NotFound, context.Exception.Message);
            else if (context.Exception is InvalidOperationException)
                context.Response = context.Request.CreateErrorResponse(HttpStatusCode.Conflict, context.Exception.Message);
        }
    }
}