﻿using Microsoft.EntityFrameworkCore;
using PiketWebApi.Data;
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

        private static IResult GetSchoolYearById(HttpContext context, ApplicationDbContext dbContext, int id)
        {
            try
            {
                var result = dbContext.SchoolYears.SingleOrDefault(x => x.Id==id);
                return Results.Ok(result);
            }
            catch (Exception ex)
            {
                return Results.BadRequest(ex.Message);
            }
        }
        private static IResult GetActiveSchoolYear(HttpContext context, ApplicationDbContext dbContext)
        {
            try
            {
                var result = dbContext.SchoolYears.SingleOrDefault(x => x.Actived);
                return Results.Ok(result);
            }
            catch (Exception ex)
            {
                return Results.BadRequest(ex.Message);
            }
        }

        private static IResult DeleteSchoolYear(HttpContext context, ApplicationDbContext dbContext, int id)
        {
            try
            {
                var result = dbContext.SchoolYears.SingleOrDefault(x => x.Id == id);
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

        private static IResult PutSchoolYear(HttpContext context, ApplicationDbContext dbContext, int id, SchoolYear teacher)
        {
            try
            {
                var result = dbContext.SchoolYears.SingleOrDefault(x => x.Id == id);
                if (result != null)
                {
                    dbContext.Entry(result).CurrentValues.SetValues(teacher);
                    dbContext.SaveChanges();
                }
                return Results.Ok(true);
            }
            catch (Exception ex)
            {
                return Results.BadRequest(ex.Message);
            }
        }

        private static async Task<IResult> PostSchoolYear(HttpContext context, ApplicationDbContext dbContext, SchoolYear teacher)
        {
            var trans = await dbContext.Database.BeginTransactionAsync();

            try
            {
                var activeData = dbContext.SchoolYears.Where(x => x.Actived);
                await activeData.ForEachAsync((x) => x.Actived = false);
                teacher.Actived=true;
                dbContext.SchoolYears.Add(teacher);
                dbContext.SaveChanges();
                await trans.CommitAsync();
                return Results.Ok(teacher);
            }
            catch (Exception ex)
            {
                if(ex.InnerException!=null && ex.InnerException.Message.Contains("duplicate"))
                        return Results.BadRequest($"Tahun Ajaran {teacher.Year} sudah ada");
                return Results.BadRequest(ex.Message);
            }
        }

        private static object GetAllSchoolYear(HttpContext context, ApplicationDbContext dbContext)
        {
            try
            {
                var result = dbContext.SchoolYears.ToList();
                return Results.Ok(result);
            }
            catch (Exception ex)
            {
                return Results.BadRequest(ex.Message);
            }
        }
    }
}
