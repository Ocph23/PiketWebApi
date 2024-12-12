using ErrorOr;
using Microsoft.AspNetCore.Mvc;

namespace PiketWebApi.Abstractions
{
    public static class ErrorOrExtentions
    {

        public static ProblemDetails CreateProblemDetail<T>(this ErrorOr<T> errors, HttpContext context)
        {
            ProblemDetails problemDetails = new ProblemDetails();
            problemDetails.Instance = $"{context.Request.Method} {context.Request.Path} ";
            problemDetails.Status = StatusCodes.Status400BadRequest;
            problemDetails.Title = " An error occured";
            problemDetails.Type = "BadRequest";
            problemDetails.Detail = errors.Errors.FirstOrDefault().Description;
            problemDetails.Extensions.Add("errors", errors.Errors);

            return problemDetails;


        }

    }
}
