
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PiketWebApi.Data;
using PiketWebApi.Models;

namespace PiketWebApi.Api
{
    public static class ClassRoomApi
    {
        public static RouteGroupBuilder MapClassRoomApi(this RouteGroupBuilder group)
        {
            group.MapGet("/", GetAllClassRoom);
            group.MapPost("/", PostClassRoom);
            group.MapPut("/{id}", PutClassRoom);
            group.MapDelete("/{id}", DeleteClassRoom);
            return group.WithTags("classroom").RequireAuthorization(); ;
        }

        private static IResult DeleteClassRoom(HttpContext context, ApplicationDbContext dbContext, int id)
        {
            try
            {
                var result = dbContext.ClassRooms.SingleOrDefault(x => x.Id == id);
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

        private static IResult PutClassRoom(HttpContext context, ApplicationDbContext dbContext, int id, ClassRoom model)
        {
            try
            {
                var result = dbContext.ClassRooms.SingleOrDefault(x=>x.Id==id);
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

        private static IResult PostClassRoom(HttpContext context, ApplicationDbContext dbContext, ClassRoom model)
        {
            try
            {
                var result = dbContext.ClassRooms.Add(model);
                dbContext.SaveChanges();
                return Results.Ok(model);
            }
            catch (Exception ex)
            {
                return Results.BadRequest(ex.Message);
            }
        }

        private static object GetAllClassRoom(HttpContext context, ApplicationDbContext dbContext)
        {
            try
            {
                var result = dbContext.ClassRooms.ToList();
                return Results.Ok(result);
            }
            catch (Exception ex)
            {
                return Results.BadRequest(ex.Message);
            }
        }
    }
}
