using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace CompanyEmployees
{
    public class globalExceptionHandler : IExceptionHandler
    {
        private readonly ILoggerManager _logger;

        public globalExceptionHandler(ILoggerManager logger)
        {
            _logger = logger;
        }

        public async ValueTask<bool> TryHandleAsync(
            HttpContext httpContext,
            Exception exception,
            CancellationToken cancellationToken)
        {
            // Log the exception first
            _logger.LogError("Unhandled exception caught by globalExceptionHandler");

            // If the response already started, we cannot change headers or write a new JSON body.
            if (httpContext.Response.HasStarted)
            {
                _logger.LogWarn("The response has already started; the exception handler cannot modify the response.");
                return false; // not handled here
            }

            // Clear anything that might have been written and set status+content-type BEFORE writing body
            httpContext.Response.Clear();
            httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
            httpContext.Response.ContentType = "application/json";

            var payload = new
            {
                status = 500,
                title = "Internal Server Error",
                detail = exception?.Message,
                // don't leak stack traces in production - optionally include when Environment.IsDevelopment()
                time = DateTime.UtcNow
            };

            await httpContext.Response.WriteAsJsonAsync(payload, cancellationToken);
            return true; // we handled the exception and wrote the response
        }
    }
}
