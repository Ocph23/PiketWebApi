
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PiketWebApi.Data;
using PiketWebApi.Models;

namespace PiketWebApi.Api
{
    public static class StudentApi
    {
        public static RouteGroupBuilder MapStudentApi(this RouteGroupBuilder group)
        {
            group.MapGet("/", GetAllStudent);
            group.MapGet("/search/{searchtext}", SearchStudent);
            group.MapPost("/", PostStudent);
            group.MapPut("/{id}", PutStudent);
            group.MapDelete("/{id}", DeleteStudent);
            return group.WithTags("student").RequireAuthorization(); ;
        }

        private static async Task<IResult> SearchStudent(HttpContext context, ApplicationDbContext dbContext, string searchtext)
        {
            try
            {
                var txtSearch = searchtext.ToLower();
                var result = dbContext.Students.Where(x => x.Name.ToLower().Contains(txtSearch)
                || x.Email.ToLower().Contains(txtSearch)
                || x.Number.ToLower().Contains(txtSearch)).ToList();
                return Results.Ok(result);
            }
            catch (Exception ex)
            {
                return Results.BadRequest(ex.Message);
            }
        }

        private static IResult DeleteStudent(HttpContext context, ApplicationDbContext dbContext, int id)
        {
            try
            {
                var result = dbContext.Students.SingleOrDefault(x => x.Id == id);
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

        private static IResult PutStudent(HttpContext context, ApplicationDbContext dbContext, int id, Student model)
        {
            try
            {
                var result = dbContext.Students.SingleOrDefault(x => x.Id == id);
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

        private static async Task<IResult> PostStudent(HttpContext context, ApplicationDbContext dbContext, UserManager<ApplicationUser> _userManager, Student model)
        {
            var trans = dbContext.Database.BeginTransaction();
            try
            {
                if (!string.IsNullOrEmpty(model.Email))
                {
                    var user = new ApplicationUser { Email = model.Email, EmailConfirmed = true, Name = model.Name, UserName = model.Email };
                    var createResult = await _userManager.CreateAsync(user, "Password@123");
                    if (createResult.Succeeded)
                    {
                        await _userManager.AddToRoleAsync(user, "Student");
                        model.UserId = user.Id;
                    }
                    else
                    {
                        throw new SystemException("User Gagal Dibuat !");
                    }
                }

                var result = dbContext.Students.Add(model);
                dbContext.SaveChanges();
                trans.Commit();
                return Results.Ok(model);
            }
            catch (Exception ex)
            {
                trans.Rollback();
                return Results.BadRequest(ex.Message);
            }
        }

        private static object GetAllStudent(HttpContext context, ApplicationDbContext dbContext)
        {
            try
            {
                var result = dbContext.Students.ToList();
                return Results.Ok(result);
            }
            catch (Exception ex)
            {
                return Results.BadRequest(ex.Message);
            }
        }
    }
}
