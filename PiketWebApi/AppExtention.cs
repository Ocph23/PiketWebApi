using DateOnlyTimeOnly.AspNet.Converters;
using ErrorOr;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PiketWebApi.Data;
using SharedModel;
using SharedModel.Models;
using System.Security.Claims;

namespace PiketWebApi
{
    public static class AppExtention
    {
        public static async Task<(bool, Teacher?)> IsTeacherPicket(this IHttpContextAccessor http, UserManager<ApplicationUser> userManager, ApplicationDbContext dbContext)
        {
            try
            {
                var userClaim = http.HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier);
                var id = userClaim?.Value.ToString();
                var user = await userManager.FindByEmailAsync(id);
                if (user == null)
                {
                    throw new UnauthorizedAccessException();
                }
                var teacher = dbContext.Teachers.SingleOrDefault(x => x.UserId == user.Id);
                if (teacher == null)
                {
                    throw new UnauthorizedAccessException();
                }
                var schedule = dbContext.Schedules.Where(x => x.DayOfWeek == DateTime.Now.DayOfWeek && x.Teacher.Id == teacher.Id).Include(x => x.Teacher);
                return (true, teacher);
            }
            catch (Exception)
            {
                return (false, null);
            }
        }

        public static List<Error> GetErrors(this ValidationResult? validateResult)
        {
            List<Error> errors = new();
            errors.Add(Error.Validation("Message", "Data tidak valid."));

            if (validateResult != null)
            {
                foreach (var item in validateResult?.Errors)
                {
                    errors.Add(Error.Validation(item.PropertyName, item.ErrorMessage));
                }
            }
            return errors;
        }

        public static ProblemDetails CreateProblemDetail<T>(this ErrorOr<T> errors, HttpContext context)
        {
            ProblemDetails problemDetails = new ProblemDetails();
            problemDetails.Instance = $"{context.Request.Method} {context.Request.Path} ";
            problemDetails.Status = StatusCodes.Status400BadRequest;
            problemDetails.Title = " An error occured";
            problemDetails.Type = "BadRequest";
            problemDetails.Detail = errors.FirstError.Description;
            problemDetails.Extensions.Add("errors", errors.Errors);

            return problemDetails;

        }

        public static IResult GetErrorResult<T>(this ErrorOr<T> errors, HttpContext context)
        {
            ProblemDetails problemDetails = new ProblemDetails();
            problemDetails.Instance = $"{context.Request.Method} {context.Request.Path} ";
            problemDetails.Status = StatusCodes.Status400BadRequest;
            problemDetails.Title = "An error occured";
            problemDetails.Type = errors.FirstError.Code;
            problemDetails.Detail = errors.FirstError.Description;
            problemDetails.Extensions.Add("errors", errors.Errors);

            if (errors.FirstError.Type == ErrorType.Unauthorized)
                problemDetails.Status = StatusCodes.Status401Unauthorized;

            return Results.BadRequest(problemDetails);
        }







    }


}
