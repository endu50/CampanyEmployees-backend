using Microsoft.AspNetCore.Diagnostics;

namespace CompanyEmployees
{
    public class globalExceptionHandler : IExceptionHandler
    {
        public async  ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
           await httpContext.Response.WriteAsJsonAsync("Something Wrong");
            httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
            return true;
                 
        }
    }
}
