
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PiketWebApi.Data;
using PiketWebApi.Models;
using PiketWebApi.Services;
using SharedModel.Models;

namespace PiketWebApi.Api
{
    public static class PickerApi
    {
        public static RouteGroupBuilder MapPickerApi(this RouteGroupBuilder group)
        {
            group.MapGet("/", GetPickerToday);
            group.MapGet("/create", PostPicket);
            group.MapPost("/createlate", Createlate);
            group.MapPost("/createsoearly", Createsoearly);
            return group.WithTags("picket").RequireAuthorization(); ;
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
