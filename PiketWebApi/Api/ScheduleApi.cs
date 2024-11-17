
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PiketWebApi.Data;
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
            group.MapGet("/active", GetScheduleBySchoolActive);
            group.MapPost("/", PostSchedule);
            group.MapPut("/{id}", PutSchedule);
            group.MapDelete("/{id}", DeleteSchedule);
            return group.WithTags("schedule").RequireAuthorization(); ;
        }

        private static async Task<IResult> GetAllScheduleBySchoolYear(HttpContext context, ApplicationDbContext dbContext, int id)
        {
            try
            {
                var result = from a in dbContext.Schedules.Where(x=>x.SchoolYear.Id==id).Include(x => x.SchoolYear).Include(x => x.Teacher)
                select new ScheduleResponse(a.Id, a.SchoolYear.Id, a.SchoolYear.Year, a.DayOfWeek.ToString(), a.Teacher.Id,
                a.Teacher.Number, a.Teacher.Name, "https://upload.wikimedia.org/wikipedia/commons/7/7c/Profile_avatar_placeholder_large.png");
                return Results.Ok(result);
            }
            catch (Exception ex)
            {
                return Results.BadRequest(ex.Message);
            }
        }

        private static async Task<IResult> GetScheduleBySchoolActive(HttpContext context, ApplicationDbContext dbContext)
        {
            try
            {

                var shoolYearActive = dbContext.SchoolYears.SingleOrDefault(x => x.Actived);
                if (shoolYearActive == null)
                    throw new SystemException("Tahun Ajaran belum ada, hubungi administrator");

                var result = from a in dbContext.Schedules.Include(x => x.SchoolYear).Include(x => x.Teacher)
                             .Where(x => x.SchoolYear.Id == shoolYearActive.Id)
                             select new ScheduleResponse(a.Id, a.SchoolYear.Id, a.SchoolYear.Year, a.DayOfWeek.ToString(), a.Teacher.Id,
                             a.Teacher.Number, a.Teacher.Name, "https://upload.wikimedia.org/wikipedia/commons/7/7c/Profile_avatar_placeholder_large.png");
                return Results.Ok(result);


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

        private static IResult PutSchedule(HttpContext context, ApplicationDbContext dbContext, int id, ScheduleRequest model)
        {
            try
            {
                var result = dbContext.Schedules.SingleOrDefault(x => x.Id == id);
                if (result != null)
                {
                    result.DayOfWeek = (DayOfWeek)model.DayOfWeek;
                    dbContext.SaveChanges();
                }
                return Results.Ok(true);
            }
            catch (Exception ex)
            {
                return Results.BadRequest(ex.Message);
            }
        }

        private static IResult PostSchedule(HttpContext context, ApplicationDbContext dbContext, ScheduleRequest req)
        {
            try
            {
                var shoolYearActive = dbContext.SchoolYears.SingleOrDefault(x => x.Actived);
                if (shoolYearActive == null)
                    throw new SystemException("Tahun Ajaran aktif belum ada, hubungi administrator");

                var existsSchedule = dbContext.Schedules
                    .Include(x => x.Teacher)
                    .Include(x => x.SchoolYear)
                    .FirstOrDefault(x => x.SchoolYear.Actived && x.Teacher.Id == req.TeacherId);


                if (existsSchedule != null)
                    throw new SystemException($"{existsSchedule.Teacher.Name} sudah terdaftar");
                var model = new Schedule
                {
                    DayOfWeek = (DayOfWeek)req.DayOfWeek,
                    SchoolYear = shoolYearActive,
                    Teacher = new Teacher { Id = req.TeacherId }
                };
                dbContext.Entry(model.Teacher).State = EntityState.Unchanged;
                var result = dbContext.Schedules.Add(model);
                dbContext.SaveChanges();


                return Results.Ok(new ScheduleResponse(model.Id, model.SchoolYear.Id, model.SchoolYear.Year, model.DayOfWeek.ToString(), model.Teacher.Id,
                             model.Teacher.Number, model.Teacher.Name));
            }
            catch (Exception ex)
            {
                return Results.BadRequest(ex.Message);
            }
        }

        private static IResult GetAllSchedule(HttpContext context, ApplicationDbContext dbContext)
        {
            try
            {
                var result = from a in dbContext.Schedules.Include(x => x.SchoolYear).Include(x => x.Teacher)
                             select new ScheduleResponse(a.Id, a.SchoolYear.Id, a.SchoolYear.Year, a.DayOfWeek.ToString(), a.Teacher.Id,
                             a.Teacher.Number, a.Teacher.Name, "https://upload.wikimedia.org/wikipedia/commons/7/7c/Profile_avatar_placeholder_large.png");
                return Results.Ok(result);
            }
            catch (Exception ex)
            {
                return Results.BadRequest(ex.Message);
            }
        }
    }
}
