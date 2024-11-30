using Microsoft.EntityFrameworkCore;
using PiketWebApi.Data;
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
            group.MapPost("/", PostClassRoom);
            group.MapPut("/{id}", PutClassRoom);
            group.MapDelete("/{id}", DeleteClassRoom);

            group.MapPost("/addstudent/{classroomId}", AddStudentToClassRoom);
            group.MapDelete("/removestudent/{classroomId}/{studentId}", RemoveStudentFromClassRoom);
            return group.WithTags("classroom").RequireAuthorization(); ;
        }
        private static async Task<IResult> PostClassRoom(HttpContext context, IClassRoomService classRoomService,  ClassRoomRequest req)
        {
            try
            {
                var result = await classRoomService.PostClassRoom(req);
                return Results.Ok(result);
    
            }
            catch (Exception)
            {
                return Results.BadRequest(Helper.ApiCommonError);
            }
        }

        private static async Task<IResult> GetClassRoomById(HttpContext context, ApplicationDbContext dbContext, IClassRoomService classRoomService, int id)
        {
            try
            {
                var data = await classRoomService.GetClassRoomById(id);
                return Results.Ok(data);
            }
            catch (Exception)
            {
                return Results.BadRequest(Helper.ApiCommonError);
            }
        }

        private static async Task<IResult> RemoveStudentFromClassRoom(HttpContext context, IClassRoomService classRoomService, int classroomId, int studentId)
        {
            try
            {
                var data = await classRoomService.RemoveStudentFromClassRoom(classroomId, studentId);
                return Results.Ok(data);
            }
            catch (Exception)
            {
                return Results.BadRequest(Helper.ApiCommonError);
            }
        }

        private static async Task<IResult> AddStudentToClassRoom(HttpContext context, IClassRoomService classRoomService, int classroomId, Student student)
        {
            try
            {
                var data = await classRoomService.AddStudentToClassRoom(classroomId, student);
                return Results.Ok(data);

            }
            catch (Exception)
            {
                return TypedResults.BadRequest(Helper.ApiCreateError);
            }
        }

        private static async Task<IResult> DeleteClassRoom(HttpContext context, IClassRoomService classRoomService, int id)
        {

            try
            {
                var data = await classRoomService.DeleteClassRoom(id);
                return Results.Ok(data);
            }
            catch (Exception)
            {
                return Results.BadRequest(Helper.ApiCommonError);
            }

        }

        private static async Task<IResult> PutClassRoom(HttpContext context, IClassRoomService classRoomService, int id, ClassRoomRequest req)
        {
            try
            {
                var data = await classRoomService.PutClassRoom(id,req);
                return Results.Ok(data);
            }
            catch (Exception)
            {
                return Results.BadRequest(Helper.ApiCreateError);
            }
        }


        private static async Task<IResult> GetAllClassRoom(HttpContext context, IClassRoomService classRoomService)
        {
            try
            {
                var data = await classRoomService.GetAllClassRoom();
                return Results.Ok(data);
            }
            catch (Exception)
            {
                return Results.BadRequest(Helper.ApiCommonError);
            }
        }
    }
}
