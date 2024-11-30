using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using PiketWebApi.Data;
using PiketWebApi.Services;
using SharedModel.Models;
using SharedModel.Requests;

namespace PiketWebApi.Api
{
    public static class PickerApi
    {
        public static RouteGroupBuilder MapPickerApi(this RouteGroupBuilder group)
        {
            group.MapGet("/", GetPickerToday);
            group.MapPost("/", PostPicket);
            group.MapPut("/{id}", PutPicket);
            group.MapPost("/late", Createlate);
            group.MapPost("/early", Createsoearly);
            group.MapDelete("/late/{id}", RemoveLate);
            group.MapDelete("/early/{id}", RemoveSoearly);
            return group.WithTags("picket").RequireAuthorization(); ;
        }

        private static async Task<IResult> PutPicket(HttpContext context, ApplicationDbContext dbContext, int id, Picket model)
        {
            try
            {
                var result = dbContext.Picket.Include(x => x.CreatedBy)
                    .SingleOrDefault(x => x.Id == id);
                if (result != null)
                {
                    result.Weather = model.Weather;
                    result.StartAt = model.StartAt;
                    result.EndAt = model.EndAt;
                    if (result.CreatedBy.Id != model.CreatedBy.Id)
                    {
                        dbContext.Entry(result.CreatedBy).State = EntityState.Unchanged;
                        result.CreatedBy = model.CreatedBy;
                    }
                    dbContext.SaveChanges();
                    return TypedResults.Ok(true);
                }
                throw new SystemException("Terjadi Kesalahan !, Coba Ulangi Lagi");
            }
            catch (Exception ex)
            {
                return TypedResults.BadRequest(ex.Message);
            }
        }

        private static async Task<IResult> RemoveSoearly(HttpContext context, ApplicationDbContext dbContext, int id)
        {
            try
            {
                var result = dbContext.Picket.Include(x => x.StudentsComeHomeEarly)
                    .Where(x => x.StudentsComeHomeEarly
                    .Where(x => x.Id == id).Any())
                    .SelectMany(x => x.StudentsComeHomeEarly).SingleOrDefault();
                if (result != null)
                {
                    dbContext.Remove(result);
                    dbContext.SaveChanges();
                    return TypedResults.Ok(true);
                }
                throw new SystemException("Terjadi Kesalahan !, Coba Ulangi Lagi");
            }
            catch (Exception ex)
            {

                return TypedResults.BadRequest(ex.Message);
            }
        }

        private static async Task<IResult> RemoveLate(HttpContext context, ApplicationDbContext dbContext, int id)
        {
            try
            {
                var result = dbContext.Picket.Include(x => x.StudentsToLate)
                    .Where(x => x.StudentsToLate
                    .Where(x => x.Id == id).Any())
                    .SelectMany(x => x.StudentsToLate).SingleOrDefault();
                if (result != null)
                {
                    dbContext.Remove(result);
                    dbContext.SaveChanges();
                    return TypedResults.Ok(true);
                }
                throw new SystemException("Terjadi Kesalahan !, Coba Ulangi Lagi");
            }
            catch (Exception ex)
            {

                return TypedResults.BadRequest(ex.Message);
            }
        }

        private static async Task<IResult> Createsoearly(HttpContext context, IPicketService picketService, StudentToLateAndEarlyRequest early)
        {
            try
            {
                var picket = await picketService.AddStudentComeHomeSoEarly(early);
                return Results.Ok(early);
            }
            catch (Exception ex)
            {
                return Results.BadRequest(ex.Message);
            }

        }

        private static async Task<IResult> Createlate(HttpContext context, IPicketService picketService, StudentToLateAndEarlyRequest late)
        {
            try
            {
                var result = await picketService.AddStudentToLate(late);
                return Results.Ok(late);
            }
            catch (Exception ex)
            {
                return Results.BadRequest(ex.Message);
            }
        }

        private static async Task<IResult> PostPicket(HttpContext context, IPicketService picketService)
        {
            try
            {
                var result = await picketService.CreateNewPicket();
                return Results.Ok(result);
            }
            catch (Exception ex)
            {
                return Results.BadRequest(ex.Message);
            }
        }

        private static async Task<IResult> GetPickerToday(HttpContext context, IPicketService picketService)
        {
            try
            {
                var result = await picketService.GetPicketToday();
                return Results.Ok(result);
            }
            catch (Exception ex)
            {
                return Results.BadRequest(ex.Message);
            }
        }
    }
}
