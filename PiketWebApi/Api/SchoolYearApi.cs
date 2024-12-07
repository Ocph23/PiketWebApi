using Microsoft.EntityFrameworkCore;
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
            try
            {
                return Results.Ok(await schoolService.GetSchoolYearById(id));
            }
            catch (Exception)
            {
                return Results.BadRequest(Helper.ApiCommonError);
            }
        }
        private static async Task<IResult> GetActiveSchoolYear(HttpContext context, ISchoolYearService schoolService)
        {
            try
            {
                return Results.Ok(await schoolService.GetActiveSchoolYear());
            }
            catch (Exception)
            {
                return Results.BadRequest(Helper.ApiCommonError);
            }
        }

        private static async Task<IResult> DeleteSchoolYear(HttpContext context, ISchoolYearService schoolService, int id)
        {
            try
            {
                return Results.Ok(await schoolService.DeleteSchoolYear(id));
            }
            catch (Exception)
            {
                return Results.BadRequest(Helper.ApiCreateError);
            }
        }

        private static async Task<IResult> PutSchoolYear(HttpContext context, ISchoolYearService schoolService, int id, SchoolYear model)
        {
            try
            {
                return Results.Ok(await schoolService.PutSchoolYear(id,model));
            }
            catch (Exception)
            {
                return Results.BadRequest(Helper.ApiCreateError);
            }
        }

        private static async Task<IResult> PostSchoolYear(HttpContext context, ISchoolYearService schoolService, SchoolYear model)
        {
            try
            {
                return Results.Ok(await schoolService.PostSchoolYear(model));
            }
            catch (Exception)
            {
                return Results.BadRequest(Helper.ApiCreateError);
            }
        }

        private static async Task<IResult> GetAllSchoolYear(HttpContext context, ISchoolYearService schoolService)
        {
            try
            {
                return Results.Ok(await schoolService.GetAllSchoolYear());
            }
            catch (Exception)
            {
                return Results.BadRequest(Helper.ApiCreateError);
            }
        }
    }
}
