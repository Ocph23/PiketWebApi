
using ErrorOr;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PiketWebApi.Abstractions;
using PiketWebApi.Data;
using PiketWebApi.Services;
using SharedModel.Models;
using SharedModel.Requests;

namespace PiketWebApi.Api
{
    public static class StudentAttendanceApi
    {
        public static RouteGroupBuilder MapStudentAttendanceApi(this RouteGroupBuilder group)
        {
            group.MapGet("/", Get);
            group.MapGet("/{id}", GetById);
            group.MapPost("/", Post);
            group.MapPut("/{id}", Put);
            group.MapDelete("/{id}", Delete);
            return group.WithTags("studentattendance").RequireAuthorization(); ;
        }

        private static async Task<IResult> Delete(HttpContext context, IStudentAttendaceService studentService, int id)
        {
            var result = await studentService.DeleteAsync(id);
            return result.Match(items => Results.Ok(items), errors => Results.BadRequest(result.CreateProblemDetail(context)));
        }
        private static async Task<IResult> Post(HttpContext context, IStudentAttendaceService studentService, StudentAttendenceRequest req)
        {
            var result = await studentService.PostAsync(req);
            return result.Match(items => Results.Ok(items), errors => Results.BadRequest(result.CreateProblemDetail(context)));
        }

        private static async Task<IResult> Put(HttpContext context, IStudentAttendaceService studentService, int id, StudentAttendenceRequest req)
        {
            var result = await studentService.PutAsync(id, req);
            return result.Match(items => Results.Ok(items), errors => Results.BadRequest(result.CreateProblemDetail(context)));
        }

        private static async Task<IResult> Get(HttpContext context, IStudentAttendaceService studentService)
        {
            var result = await studentService.GetAsync();
            return result.Match(items => Results.Ok(items), errors => Results.BadRequest(result.CreateProblemDetail(context)));
        }

        private static async Task<IResult> GetById(HttpContext context, IStudentService studentService, int id)
        {
            var result = await studentService.GetStudentById(id);
            return result.Match(items => Results.Ok(items), errors => Results.BadRequest(result.CreateProblemDetail(context)));
        }
    }
}
