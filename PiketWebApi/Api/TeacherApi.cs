
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PiketWebApi.Data;
using PiketWebApi.Services;
using SharedModel;
using SharedModel.Models;
using System.Linq;
using System.Net;

namespace PiketWebApi.Api
{
    public static class TeacherApi
    {
        public static RouteGroupBuilder MapTeacherApi(this RouteGroupBuilder group)
        {
            group.MapGet("/", GetAllTeacher);
            group.MapGet("/{id}", GetTeacherById);
            group.MapGet("/search/{searchtext}", SearchTeacher);
            group.MapPost("/", PostTeacher);
            group.MapPut("/{id}", PutTeacher);
            group.MapDelete("/{id}", DeleteTeacher);
            group.MapPut("/photo/{id}", UploadFoto);
            return group.WithTags("teacher").RequireAuthorization(); ;
        }

        private static async Task<IResult> UploadFoto(HttpContext context, ITeacherService teacherService, int id, byte[] data)
        {
            var result = await teacherService.UploadPhoto(id,data);
            return result.Match(items => Results.Ok(items), errors => Results.BadRequest(result.CreateProblemDetail(context)));

        }

        private static async Task<IResult> SearchTeacher(HttpContext context, ITeacherService teacherService, string searchtext)
        {
            var result = await teacherService.SearchTextAsync(searchtext);
            return result.Match(items => Results.Ok(items), errors => Results.BadRequest(result.CreateProblemDetail(context)));
        }

        private static async Task<IResult> DeleteTeacher(HttpContext context, ITeacherService teacherService, int id)
        {
            var result = await teacherService.DeleteAsync(id);
            return result.Match(items => Results.Ok(items), errors => Results.BadRequest(result.CreateProblemDetail(context)));
        }

        private static async Task<IResult> PutTeacher(HttpContext context, ITeacherService teacherService, int id, Teacher teacher)
        {
            var result = await teacherService.PutAsync(id, teacher);
            return result.Match(items => Results.Ok(items), errors => Results.BadRequest(result.CreateProblemDetail(context)));
        }

        private static async Task<IResult> PostTeacher(HttpContext context, ITeacherService teacherService, Teacher teacher)
        {
            var result = await teacherService.PostAsync(teacher);
            return result.Match(items => Results.Ok(items), errors => Results.BadRequest(result.CreateProblemDetail(context)));
        }

        private static async Task<IResult> GetAllTeacher(HttpContext context, ITeacherService teacherService)
        {
            var result = await teacherService.GetAsync();
            return result.Match(items => Results.Ok(items), errors => Results.BadRequest(result.CreateProblemDetail(context)));
        }

        private static async Task<IResult> GetTeacherById(HttpContext context, ITeacherService teacherService, int id)
        {
            var result = await teacherService.GetByIdAsync(id);
            return result.Match(items => Results.Ok(items), errors => Results.BadRequest(result.CreateProblemDetail(context)));
        }
    }
}
