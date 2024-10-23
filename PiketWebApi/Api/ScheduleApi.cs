
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PiketWebApi.Data;
using PiketWebApi.Models;

namespace PiketWebApi.Api
{
    public static class ScheduleApi
    {
        public static RouteGroupBuilder MapScheduleApi(this RouteGroupBuilder group)
        {
            group.MapGet("/", GetAllSchedule);
            group.MapGet("/byschoolyear/{id}", GetAllScheduleBySchoolYear);
            group.MapPost("/", PostSchedule);
            group.MapPut("/{id}", PutSchedule);
            group.MapDelete("/{id}", DeleteSchedule);
            return group.WithTags("schedule").RequireAuthorization(); ;
        }

        private static async Task<IResult> GetAllScheduleBySchoolYear(HttpContext context, ApplicationDbContext dbContext, int schoolyearId)
        {
            try
            {
                var result = dbContext.Schedules.Where(x => x.SchoolYear.Id == schoolyearId)
                    .Include(x => x.SchoolYear).ToList();
                return Results.Ok(result);
            }
            catch (Exception ex)
            {
                return Results.BadRequest(ex.Message);
            }
        }

        private static IResult DeleteSchedule(HttpContext context, ApplicationDbContext dbContext, int id)
        {
            try
            {
                var result = dbContext.Schedules.SingleOrDefault(x => x.Id == id);
                if (result != null)
                {
                    dbContext.Remove(result);
                    dbContext.SaveChanges();
                }
                return Results.Ok(true);
            }
            catch (Exception ex)
            {
                return Results.BadRequest(ex.Message);
            }
        }

        private static IResult PutSchedule(HttpContext context, ApplicationDbContext dbContext, int id, Schedule model)
        {
            try
            {
                var result = dbContext.Schedules.SingleOrDefault(x => x.Id == id);
                if (result != null)
                {
                    dbContext.Entry(result).CurrentValues.SetValues(model);
                    dbContext.SaveChanges();
                }
                return Results.Ok(true);
            }
            catch (Exception ex)
            {
                return Results.BadRequest(ex.Message);
            }
        }

        private static IResult PostSchedule(HttpContext context, ApplicationDbContext dbContext, Schedule model)
        {
            try
            {
                var result = dbContext.Schedules.Add(model);
                dbContext.SaveChanges();
                return Results.Ok(model);
            }
            catch (Exception ex)
            {
                return Results.BadRequest(ex.Message);
            }
        }

        private static object GetAllSchedule(HttpContext context, ApplicationDbContext dbContext)
        {
            try
            {
                var result = dbContext.Schedules.ToList();
                return Results.Ok(result);
            }
            catch (Exception ex)
            {
                return Results.BadRequest(ex.Message);
            }
        }
    }
}
