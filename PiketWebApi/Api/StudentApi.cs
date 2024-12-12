
using ErrorOr;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PiketWebApi.Abstractions;
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
            var result = await studentService.GetStudentWithClass(id);
            return result.Match(items => Results.Ok(items), errors => Results.BadRequest(result.CreateProblemDetail(context)));

        }

        private static async Task<IResult> GetAllStudentWithClass(HttpContext context, IStudentService studentService)
        {
            var result = await studentService.GetAlStudentWithClass();
            return result.Match(items => Results.Ok(items), errors => Results.BadRequest(result.CreateProblemDetail(context)));
        }

        private static async Task<IResult> SearchStudent(HttpContext context, IStudentService studentService, string searchtext)
        {
            var result = await studentService.SearchStudent(searchtext);
            return result.Match(items => Results.Ok(items), errors => Results.BadRequest(result.CreateProblemDetail(context)));

        }

        private static async Task<IResult> DeleteStudent(HttpContext context, IStudentService studentService, int id)
        {
            var result = await studentService.DeleteStudent(id);
            return result.Match(items => Results.Ok(items), errors => Results.BadRequest(result.CreateProblemDetail(context)));
        }

        private static async Task<IResult> PutStudent(HttpContext context, IStudentService studentService, int id, Student model)
        {
            var result = await studentService.PutStudent(id, model);
            return result.Match(items => Results.Ok(items), errors => Results.BadRequest(result.CreateProblemDetail(context)));
        }

        private static async Task<IResult> PostStudent(HttpContext context, IStudentService studentService, Student model)
        {
            var result = await studentService.PostStudent(model);
            return result.Match(items => Results.Ok(items), errors => Results.BadRequest(result.CreateProblemDetail(context)));
        }

        private static async Task<IResult> GetAllStudent(HttpContext context, IStudentService studentService)
        {

            var result = await studentService.GetAllStudent();
            return result.Match(items => Results.Ok(items), errors => Results.BadRequest(result.CreateProblemDetail(context)));
        }

        private static async Task<IResult> GetStudentById(HttpContext context, IStudentService studentService, int id)
        {
            var result = await studentService.GetStudentById(id);
            return result.Match(items => Results.Ok(items), errors => Results.BadRequest(result.CreateProblemDetail(context)));
        }
    }
}
