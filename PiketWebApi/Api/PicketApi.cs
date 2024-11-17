using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using PiketWebApi.Data;
using PiketWebApi.Services;
using SharedModel.Models;

namespace PiketWebApi.Api
{
    public static class PickerApi
    {
        public static RouteGroupBuilder MapPickerApi(this RouteGroupBuilder group)
        {
            group.MapGet("/", GetPickerToday);
            group.MapPost("/", PostPicket);
            group.MapPut("/{id}", PutPicket);
            group.MapPost("/createlate", Createlate);
            group.MapPost("/createsoearly", Createsoearly);
            group.MapDelete("/createlate/{id}", RemoveLate);
            group.MapDelete("/createsoearly/{id}", RemoveSoearly);
            return group.WithTags("picket").RequireAuthorization(); ;
        }

        private static async Task<IResult> PutPicket(HttpContext context, ApplicationDbContext dbContext, int id, Picket model)
        {
            try
            {
                var result = dbContext.Picket .Include(x=>x.CreatedBy)
                    .SingleOrDefault(x => x.Id == id);
                if (result != null)
                {
                    result.Weather = model.Weather;
                    result.StartAt = model.StartAt;
                    result.EndAt= model.EndAt;
                    result.CreatedBy= model.CreatedBy;
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

        private static async Task<IResult> Createsoearly(HttpContext context, ApplicationDbContext dbContext,
            IPicketService picketService, StudentComeHomeEarly early)
        {
            try
            {
                var picket = await picketService.GetPicketToday();
                dbContext.Entry(picket);
                picket.StudentsComeHomeEarly.Add(early);
                dbContext.SaveChanges();
                return Results.Ok(early);
            }
            catch (Exception ex)
            {
                return Results.BadRequest(ex.Message);
            }

        }

        private static async Task<IResult> Createlate(HttpContext context, ApplicationDbContext dbContext,
            IPicketService picketService, StudentToLate late)
        {
            try
            {
                var picket = await picketService.GetPicketToday();
                dbContext.Entry(picket);
                picket.StudentsToLate.Add(late);
                dbContext.SaveChanges();
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
