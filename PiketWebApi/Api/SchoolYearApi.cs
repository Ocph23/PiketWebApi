using ErrorOr;
using Microsoft.EntityFrameworkCore;
using PiketWebApi.Abstractions;
using PiketWebApi.Data;
using PiketWebApi.Services;
using SharedModel.Models;

namespace PiketWebApi.Api
{
    public static class SchoolYearApi
    {
        public static RouteGroupBuilder MapSchoolYearApi(this RouteGroupBuilder group)
        {
            group.MapGet("/", GetAllSchoolYear);
            group.MapGet("/{id}", GetSchoolYearById);
            group.MapGet("/active", GetActiveSchoolYear);
            group.MapPost("/", PostSchoolYear);
            group.MapPut("/{id}", PutSchoolYear);
            group.MapDelete("/{id}", DeleteSchoolYear);
            return group.WithTags("schoolyear").RequireAuthorization(); ;
        }

        private static async Task<IResult> GetSchoolYearById(HttpContext context, ISchoolYearService schoolService, int id)
        {

            var result = await schoolService.GetSchoolYearById(id);
            return result.Match(items => Results.Ok(items), errors => Results.BadRequest(result.CreateProblemDetail(context)));
        }
        private static async Task<IResult> GetActiveSchoolYear(HttpContext context, ISchoolYearService schoolService)
        {
            var result = await schoolService.GetActiveSchoolYear();
            return result.Match(items => Results.Ok(items), errors => Results.BadRequest(result.CreateProblemDetail(context)));
        }

        private static async Task<IResult> DeleteSchoolYear(HttpContext context, ISchoolYearService schoolService, int id)
        {
            var result = await schoolService.DeleteSchoolYear(id);
            return result.Match(items => Results.Ok(items), errors => Results.BadRequest(result.CreateProblemDetail(context)));

        }

        private static async Task<IResult> PutSchoolYear(HttpContext context, ISchoolYearService schoolService, int id, SchoolYear model)
        {
            var result = await schoolService.PutSchoolYear(id, model);
            return result.Match(items => Results.Ok(items), errors => Results.BadRequest(result.CreateProblemDetail(context)));
        }

        private static async Task<IResult> PostSchoolYear(HttpContext context, ISchoolYearService schoolService, SchoolYear model)
        {
            var result = await schoolService.PostSchoolYear(model);
            return result.Match(items => Results.Ok(items), errors => Results.BadRequest(result.CreateProblemDetail(context)));
        }

        private static async Task<IResult> GetAllSchoolYear(HttpContext context, ISchoolYearService schoolService)
        {
            var result = await schoolService.GetAllSchoolYear();
            return result.Match(items => Results.Ok(items), errors => Results.BadRequest(result.CreateProblemDetail(context)));
        }
    }
}
