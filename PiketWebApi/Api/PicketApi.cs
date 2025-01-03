using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PiketWebApi.Data;
using PiketWebApi.Services;
using SharedModel.Models;
using SharedModel.Requests;
using SharedModel.Responses;

namespace PiketWebApi.Api
{
    public static class PickerApi
    {
        public static RouteGroupBuilder MapPickerApi(this RouteGroupBuilder group)
        {
            group.MapGet("/", GetPickerToday);
            group.MapPost("/", PostPicket);
            group.MapPut("/{id}", PutPicket);
            group.MapPost("/lateandearly", AddLateandearly);
            group.MapDelete("/lateandearly/{id}", RemoveLateandearly);
            return group.WithTags("picket").RequireAuthorization(); ;
        }

        private static async Task<IResult> PutPicket(HttpContext context, IPicketService picketService, int id, PicketRequest picket)
        {
            var result = await picketService.UpdatePicket(id, picket);
            return result.Match(items => Results.Ok(items), errors => Results.BadRequest(result.CreateProblemDetail(context)));
        }

        private static async Task<IResult> GetPickerToday(HttpContext context, IPicketService picketService)
        {
            var result = await picketService.GetPicketToday();
            return result.Match(items => Results.Ok(items), errors => Results.BadRequest(result.CreateProblemDetail(context)));

        }

        //private static async Task<IResult> PutPicket(HttpContext context, IPicketService picketService, int id, Picket model)
        //{

        //    var result = await picketService.GetAsync();
        //    return result.Match(items => Results.Ok(items), errors => Results.BadRequest(result.CreateProblemDetail(context)));

        //    try
        //    {
        //        var result = dbContext.Picket.Include(x => x.CreatedBy)
        //            .SingleOrDefault(x => x.Id == id);
        //        if (result != null)
        //        {
        //            result.Weather = model.Weather;
        //            result.StartAt = model.StartAt;
        //            result.EndAt = model.EndAt;
        //            if (result.CreatedBy.Id != model.CreatedBy.Id)
        //            {
        //                dbContext.Entry(result.CreatedBy).State = EntityState.Unchanged;
        //                result.CreatedBy = model.CreatedBy;
        //            }
        //            dbContext.SaveChanges();
        //            return TypedResults.Ok(true);
        //        }
        //        throw new SystemException("Terjadi Kesalahan !, Coba Ulangi Lagi");
        //    }
        //    catch (Exception ex)
        //    {
        //        return TypedResults.BadRequest(ex.Message);
        //    }
        //}


        private static async Task<IResult> RemoveLateandearly(HttpContext context, IPicketService picketService, int id)
        {
            var result = await picketService.RemoveStudentToLateComeHomeSoEarly(id);
            return result.Match(items => Results.Ok(items), errors => Results.BadRequest(result.CreateProblemDetail(context)));
        }

     
        private static async Task<IResult> AddLateandearly(HttpContext context, IPicketService picketService, StudentToLateAndEarlyRequest model)
        {
            var result = await picketService.AddStudentToLateComeHomeSoEarly(model);
            return result.Match(items => Results.Ok(items), errors => Results.BadRequest(result.CreateProblemDetail(context)));

        }

        private static async Task<IResult> PostPicket(HttpContext context, IPicketService picketService)
        {
            var result = await picketService.CreateNewPicket();
            return result.Match(items => Results.Ok(items),  errors => result.GetErrorResult(context));
        }

    }
}
