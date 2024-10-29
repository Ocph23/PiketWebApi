
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PiketWebApi.Data;
using PiketWebApi.Models;
using PiketWebApi.Services;

namespace PiketWebApi.Api
{
    public static class PickerApi
    {
        public static RouteGroupBuilder MapPickerApi(this RouteGroupBuilder group)
        {
            group.MapGet("/", GetPickerToday);
            group.MapGet("/create", PostPicker);
            return group.WithTags("picket").RequireAuthorization(); ;
        }

        private static async Task<IResult> PostPicker(HttpContext context, IPicketService picketService)
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
