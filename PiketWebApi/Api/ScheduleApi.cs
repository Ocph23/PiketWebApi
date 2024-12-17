
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
    public static class ScheduleApi
    {
        public static RouteGroupBuilder MapScheduleApi(this RouteGroupBuilder group)
        {
            group.MapGet("/", GetAllSchedule);
            group.MapGet("/byschoolyearId/{id}", GetAllScheduleBySchoolYear);
            group.MapPost("/", PostSchedule);
            group.MapPut("/{id}", PutSchedule);
            group.MapDelete("/{id}", DeleteSchedule);
            return group.WithTags("schedule").RequireAuthorization(); ;
        }
        private static async Task<IResult> GetAllSchedule(HttpContext context, IScheduleService scheduleService)
        {
            var result = await scheduleService.GetAsync();
            return result.Match(items => Results.Ok(items), errors => Results.BadRequest(result.CreateProblemDetail(context)));
        }

        private static async Task<IResult> GetAllScheduleBySchoolYear(HttpContext context, IScheduleService scheduleService, int id)
        {
            var result = await  scheduleService.GetBySchoolYearAsync(id);
            return result.Match(items => Results.Ok(items), errors => Results.BadRequest(result.CreateProblemDetail(context)));
        }

        private static async Task<IResult>DeleteSchedule(HttpContext context, IScheduleService scheduleService, int id)
        {
            var result = await scheduleService.DeleteAsync(id);
            return result.Match(items => Results.Ok(items), errors => Results.BadRequest(result.CreateProblemDetail(context)));


        }

        private static async Task<IResult>PutSchedule(HttpContext context, IScheduleService scheduleService, int id, ScheduleRequest model)
        {
            var result = await scheduleService.PutAsync(id, model);
            return result.Match(items => Results.Ok(items), errors => Results.BadRequest(result.CreateProblemDetail(context)));

            
        }

        private static async Task<IResult>PostSchedule(HttpContext context, IScheduleService scheduleService, ScheduleRequest req)
        {
            var result = await scheduleService.PostAsync(req);
            return result.Match(items => Results.Ok(items), errors => Results.BadRequest(result.CreateProblemDetail(context)));


        }

    }
}
