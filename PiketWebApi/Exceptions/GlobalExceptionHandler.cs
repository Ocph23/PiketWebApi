using Microsoft.AspNetCore.Diagnostics;

namespace PiketWebApi.Exceptions
{
    public sealed class GlobalExceptionHandler(IProblemDetailsService probleDetailService) : IExceptionHandler
    {
        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            httpContext.Response.StatusCode = exception switch
            {
                ApplicationException => StatusCodes.Status400BadRequest,
                _ => StatusCodes.Status500InternalServerError
            };




            var problemDetails = new Microsoft.AspNetCore.Mvc.ProblemDetails()
            {
                Detail = exception.Message,
                Type = exception.GetType().Name,
                Title = " An error occured",
                Instance = $"{ httpContext.Request.Method} {httpContext.Request.Path}"
            };
            problemDetails.Extensions.Add("errors", null);
            return await probleDetailService.TryWriteAsync(new ProblemDetailsContext
            {
                Exception = exception,
                HttpContext = httpContext,
                ProblemDetails = problemDetails
            });

        }
    }
}
