
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
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace PiketWebApi.Api
{
    public static class StudentAttendanceApi
    {
        public static RouteGroupBuilder MapStudentAttendanceApi(this RouteGroupBuilder group)
        {
            group.MapGet("/", Get);
            group.MapGet("/{id}", GetById);
            group.MapGet("/{classroom}/{month}/{year}", GetByClassRoomAndMonthYear);
            group.MapPost("/", Post);
            group.MapPut("/{id}", Put);
            group.MapDelete("/{id}", Delete);
            group.MapPost("/sync", SyncData);
            return group.WithTags("studentattendance").RequireAuthorization(); ;
        }

        private static async Task<IResult> GetByClassRoomAndMonthYear(HttpContext context, IStudentAttendaceService studentService, int classroom, int month, int year)
        {
            var result = await studentService.GetAbsenByClassRoomMonthYear(classroom,month,year);
            return result.Match(items => Results.Ok(items), errors => Results.BadRequest(result.CreateProblemDetail(context)));
        }

        private static async Task<IResult> SyncData(HttpContext context, IStudentAttendaceService studentService, List<StudentAttendanceSyncRequest> data)
        {
            var result = await studentService.SyncData(data);
            return result.Match(items => Results.Ok(items), errors => Results.BadRequest(result.CreateProblemDetail(context)));
        }
        private static async Task<IResult> Delete(HttpContext context, IStudentAttendaceService studentService, Guid id)
        {
            var result = await studentService.DeleteAsync(id);
            return result.Match(items => Results.Ok(items), errors => Results.BadRequest(result.CreateProblemDetail(context)));
        }
        private static async Task<IResult> Post(HttpContext context, IStudentAttendaceService studentService, StudentAttendenceRequest req)
        {
            var result = await studentService.PostAsync(req);
            return result.Match(items => Results.Ok(items), errors => Results.BadRequest(result.CreateProblemDetail(context)));
        }

        private static async Task<IResult> Put(HttpContext context, IStudentAttendaceService studentService, Guid id, StudentAttendenceRequest req)
        {
            var result = await studentService.PutAsync(id, req);
            return result.Match(items => Results.Ok(items), errors => Results.BadRequest(result.CreateProblemDetail(context)));
        }

        private static async Task<IResult> Get(HttpContext context, IStudentAttendaceService studentService)
        {
            var result = await studentService.GetAsync();
            return result.Match(items => Results.Ok(items), errors => Results.BadRequest(result.CreateProblemDetail(context)));
        }

        private static async Task<IResult> GetById(HttpContext context, IStudentAttendaceService studentService, Guid id)
        {
            var result = await studentService.GetByIdAsync(id);
            return result.Match(items => Results.Ok(items), errors => Results.BadRequest(result.CreateProblemDetail(context)));
        }
    }
}
