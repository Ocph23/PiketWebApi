

using ErrorOr;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PiketWebApi.Data;
using PiketWebApi.Exceptions;

namespace PiketWebApi
{
    public class Helper
    {
        public static string ApiCreateError => "Maaf terjadi kesalahan, coba lengkapi data dan coba ulangi lagi.";
        public static string ApiCommonError => "Maaf terjadi kesalahan, coba ulangi lagi.";

        public static ProblemDetails CreateBadRequestProbleDetail(HttpContext context, Exception? ex)
        {
            ProblemDetails problemDetails = new ProblemDetails();
            problemDetails.Instance = $"{context.Request.Method} {context.Request.Path} ";
            problemDetails.Status = StatusCodes.Status400BadRequest;
            problemDetails.Title = " An error occured";
            problemDetails.Type = ex.GetType().Name;
            problemDetails.Detail = ex.Message;
            if (ex != null && ex.GetType() == typeof(BadRequestException))
            {
                BadRequestException badRequest = (BadRequestException)ex;
                problemDetails.Extensions.Add("errors", badRequest.Errors);
            }
            return problemDetails;
        }

        public static ProblemDetails? CreateBadRequestProbleDetail(HttpContext context, List<Error> errors)
        {
            ProblemDetails problemDetails = new ProblemDetails();
            problemDetails.Instance = $"{context.Request.Method} {context.Request.Path} ";
            problemDetails.Status = StatusCodes.Status400BadRequest;
            problemDetails.Title = " An error occured";
            problemDetails.Type = "BadRequest";
            problemDetails.Detail = errors.FirstOrDefault().Description;
            problemDetails.Extensions.Add("errors", errors);

            return problemDetails;
        }

        public static async Task<ErrorOr<ApplicationUser>> CreateUser(UserManager<ApplicationUser> userManager,  ApplicationUser user, string role)
        {
            //var user = new ApplicationUser { Email = model.Email, EmailConfirmed = true, Name = model.Name, UserName = model.Email };
            var createResult = await userManager.CreateAsync(user, "Password@123");
            if (createResult.Succeeded)
            {
                await userManager.AddToRoleAsync(user, role);
                return await Task.FromResult(user);
            }
            return Error.Failure("Failure", "User gagal dibuat !");
        }



    }
}
