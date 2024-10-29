
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PiketWebApi.Data;
using PiketWebApi.Models;

namespace PiketWebApi.Api
{
    public static class TeacherApi
    {
        public static RouteGroupBuilder MapTeacherApi(this RouteGroupBuilder group)
        {
            group.MapGet("/", GetAllTeacher);
            group.MapGet("/search/{searchtext}", SearchTeacher);
            group.MapPost("/", PostTeacher);
            group.MapPut("/{id}", PutTeacher);
            group.MapDelete("/{id}", DeleteTeacher);
            return group.WithTags("teacher").RequireAuthorization(); ;
        }


        private static async Task<IResult> SearchTeacher(HttpContext context, ApplicationDbContext dbContext, string searchtext)
        {
            try
            {
                var txtSearch = searchtext.ToLower();
                var result = dbContext.Teachers.Where(x => x.Name.ToLower().Contains(txtSearch) 
                || x.Email.ToLower().Contains(txtSearch)
                || x.Number.ToLower().Contains(txtSearch)).ToList();
             
                return Results.Ok(result);
            }
            catch (Exception ex)
            {
                return Results.BadRequest(ex.Message);
            }
        }

        private static IResult DeleteTeacher(HttpContext context, ApplicationDbContext dbContext, int id)
        {
            try
            {
                var result = dbContext.Teachers.SingleOrDefault(x => x.Id == id);
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

        private static IResult PutTeacher(HttpContext context, ApplicationDbContext dbContext, int id, Teacher teacher)
        {
            try
            {
                var result = dbContext.Teachers.SingleOrDefault(x => x.Id == id);
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

        private static async Task<IResult> PostTeacher(HttpContext context, ApplicationDbContext dbContext, UserManager<ApplicationUser> _userManager, Teacher teacher)
        {
            var trans = dbContext.Database.BeginTransaction();
            try
            {
                var user = new ApplicationUser { Email = teacher.Email, EmailConfirmed = true, Name = teacher.Name, UserName = teacher.Email };
                var createResult = await _userManager.CreateAsync(user, "Password@123");
                if (createResult.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, "Teacher");
                    teacher.UserId = user.Id;
                }
                else
                {
                    throw new SystemException("User Gagal Dibuat !");
                }

                var result = dbContext.Teachers.Add(teacher);
                dbContext.SaveChanges();

                trans.Commit();

                return Results.Ok(teacher);
            }
            catch (Exception ex)
            {
                trans.Rollback();
                return Results.BadRequest(ex.Message);
            }
        }

        private static object GetAllTeacher(HttpContext context, ApplicationDbContext dbContext)
        {
            try
            {
                var result = dbContext.Teachers.ToList();
                return Results.Ok(result);
            }
            catch (Exception ex)
            {
                return Results.BadRequest(ex.Message);
            }
        }
    }
}
