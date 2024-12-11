
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PiketWebApi.Data;
using PiketWebApi.Services;
using SharedModel.Models;

namespace PiketWebApi.Api
{
    public static class StudentApi
    {
        public static RouteGroupBuilder MapStudentApi(this RouteGroupBuilder group)
        {
            group.MapGet("/", GetAllStudent);
            group.MapGet("/withclass", GetAllStudentWithClass);
            group.MapGet("/withclas/{id}", GetStudentWithClass);
            group.MapGet("/{id}", GetStudentById);
            group.MapGet("/search/{searchtext}", SearchStudent);
            group.MapPost("/", PostStudent);
            group.MapPut("/{id}", PutStudent);
            group.MapDelete("/{id}", DeleteStudent);
            return group.WithTags("student").RequireAuthorization(); ;
        }

        private static async Task<IResult> GetStudentWithClass(HttpContext context, IStudentService studentService, int id)
        {
            try
            {
                return Results.Ok(await studentService.GetStudentWithClass(id));
            }
            catch (Exception)
            {
                return Results.BadRequest(Helper.ApiCommonError);
            }
        }

        private static async Task<IResult> GetAllStudentWithClass(HttpContext context,IStudentService studentService)
        {
            try
            {
               return Results.Ok(await studentService.GetAlStudentWithClass());
            }
            catch (Exception)
            {
                return Results.BadRequest(Helper.ApiCommonError);
            }
        }

        private static async Task<IResult> SearchStudent(HttpContext context, IStudentService studentService, string searchtext)
        {
            try
            {
                return Results.Ok(await studentService.SearchStudent(searchtext));
            }
            catch (Exception ex)
            {
                return Results.BadRequest(Helper.ApiCommonError);
            }
        }

        private static async Task<IResult> DeleteStudent(HttpContext context, IStudentService studentService, int id)
        {
            try
            {
                return Results.Ok(await studentService.DeleteStudent(id));
            }
            catch (Exception ex)
            {
                return Results.BadRequest(Helper.ApiCreateError);
            }
        }

        private static async Task<IResult> PutStudent(HttpContext context, IStudentService studentService, int id, Student model)
        {
            try
            {
                return Results.Ok(await studentService.PutStudent(id,model));
            }
            catch (Exception ex)
            {
                return Results.BadRequest(Helper.ApiCreateError);
            }
        }

        private static async Task<IResult> PostStudent(HttpContext context, IStudentService studentService, Student model)
        {
            try
            {
                return Results.Ok(await studentService.PostStudent(model));
            }
            catch (Exception ex)
            {
                return Results.BadRequest(Helper.ApiCreateError);
            }
        }

        private static async Task<IResult> GetAllStudent(HttpContext context, IStudentService studentService)
        {
            try
            {
                return Results.Ok(await studentService.GetAllStudent());
            }
            catch (Exception ex)
            {
                return Results.BadRequest(Helper.ApiCommonError);
            }
        }

        private static async Task<IResult> GetStudentById(HttpContext context, IStudentService studentService, int id)
        {
            try
            {
                return Results.Ok(await studentService.GetStudentById(id));
            }
            catch (Exception ex)
            {
                return Results.BadRequest(Helper.ApiCreateError);
            }
        }
    }
}
