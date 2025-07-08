using ErrorOr;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PiketWebApi.Abstractions;
using PiketWebApi.Data;
using PiketWebApi.Exceptions;
using PiketWebApi.Services;
using SharedModel.Models;
using SharedModel.Requests;
using SharedModel.Responses;

namespace PiketWebApi.Api
{
    public static class ClassRoomApi
    {
        public static RouteGroupBuilder MapClassRoomApi(this RouteGroupBuilder group)
        {
            group.MapGet("/", GetAllClassRoom);
            group.MapGet("/{id}", GetClassRoomById);
            group.MapGet("/schoolyear/{id}", GetClassRoomBySchoolYear);
            group.MapGet("/byteacher", GetClassRoomByTeacher);
            group.MapPost("/", PostClassRoom);
            group.MapPost("/classroomfromlast", CreateClassRoomFromLastClass);
            group.MapPut("/{id}", PutClassRoom);
            group.MapDelete("/{id}", DeleteClassRoom);
            group.MapPost("/addstudent/{classroomId}", AddStudentToClassRoom);
            group.MapDelete("/removestudent/{classroomId}/{studentId}", RemoveStudentFromClassRoom);
            return group.WithTags("classroom").RequireAuthorization(); ;
        }

        private static async Task<IResult> GetClassRoomByTeacher(HttpContext context, IClassRoomService classRoomService, ITeacherService teacherService)
        {
            var userId = context.User.Claims.FirstOrDefault(x=>x.Type=="id");
            var teacher = await teacherService.GetByUserIdAsync(userId.Value);
            if (!teacher.IsError)
            {
                var result = await classRoomService.GetClassRoomByTeacherId(teacher.Value.Id);
                return result.Match(items => Results.Ok(items), errors => Results.BadRequest(result.CreateProblemDetail(context)));
            }
            return Results.BadRequest(teacher.CreateProblemDetail(context));
        }

        private static async Task<IResult> CreateClassRoomFromLastClass(HttpContext context, IClassRoomService classRoomService, ClassRoomFromLastClassRequest req)
        {
            var result = await classRoomService.CreateClassRoomFromLastClass(req);
            return result.Match(items => Results.Ok(items), errors => Results.BadRequest(result.CreateProblemDetail(context)));

        }

        private static async Task<IResult> GetClassRoomBySchoolYear(HttpContext context, IClassRoomService classRoomService, int id)
        {
            var result = await classRoomService.GetClassRoomBySchoolYear(id);
            return result.Match(items => Results.Ok(items), errors => Results.BadRequest(result.CreateProblemDetail(context)));
        }

        private static async Task<IResult> PostClassRoom(HttpContext context, IClassRoomService classRoomService, ClassRoomRequest req)
        {
            var result = await classRoomService.PostClassRoom(req);
            return result.Match(items => Results.Ok(items), errors => Results.BadRequest(result.CreateProblemDetail(context)));
        }

        private static async Task<IResult> GetClassRoomById(HttpContext context, ApplicationDbContext dbContext, IClassRoomService classRoomService, int id)
        {
            var result = await classRoomService.GetClassRoomById(id);
            return result.Match(items => Results.Ok(items), errors => Results.BadRequest(result.CreateProblemDetail(context)));
        }

        private static async Task<IResult> RemoveStudentFromClassRoom(HttpContext context, IClassRoomService classRoomService, int classroomId, int studentId)
        {
            var result = await classRoomService.RemoveStudentFromClassRoom(classroomId, studentId);
            return result.Match(items => Results.Ok(items), errors => Results.BadRequest(result.CreateProblemDetail(context)));
        }

        private static async Task<IResult> AddStudentToClassRoom(HttpContext context, IClassRoomService classRoomService, int classroomId, Student student)
        {
            var result = await classRoomService.AddStudentToClassRoom(classroomId, student);
            return result.Match(items => Results.Ok(items), errors => Results.BadRequest(result.CreateProblemDetail(context)));
        }

        private static async Task<IResult> DeleteClassRoom(HttpContext context, IClassRoomService classRoomService, int id)
        {
            var result = await classRoomService.DeleteClassRoom(id);
            return result.Match(items => Results.Ok(items), errors => Results.BadRequest(result.CreateProblemDetail(context)));
        }

        private static async Task<IResult> PutClassRoom(HttpContext context, IClassRoomService classRoomService, int id, ClassRoomRequest req)
        {
            var result = await classRoomService.PutClassRoom(id, req);
            return result.Match(items => Results.Ok(items), errors => Results.BadRequest(result.CreateProblemDetail(context)));
        }


        private static async Task<IResult> GetAllClassRoom(HttpContext context, IClassRoomService classRoomService)
        {
            var result = await classRoomService.GetAllClassRoom();
            return result.Match(items => Results.Ok(items), errors => Results.BadRequest(result.CreateProblemDetail(context)));
        }
    }
}
