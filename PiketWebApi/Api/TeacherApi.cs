
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PiketWebApi.Data;
using PiketWebApi.Services;
using SharedModel;
using SharedModel.Models;

namespace PiketWebApi.Api
{
    public static class TeacherApi
    {
        public static RouteGroupBuilder MapTeacherApi(this RouteGroupBuilder group)
        {
            group.MapGet("/", GetAllTeacher);
            group.MapGet("/{id}", GetTeacherById);
            group.MapPost("/", PostTeacher);
            group.MapPut("/{id}", PutTeacher);
            group.MapDelete("/{id}", DeleteTeacher);
            return group.WithTags("teacher").RequireAuthorization(); ;
        }

        private static async Task<IResult> DeleteTeacher(HttpContext context, ITeacherService teacherService, int id)
        {
            try
            {
                return Results.Ok(await teacherService.Delete(id));
            }
            catch (Exception)
            {
                return Results.BadRequest(Helper.ApiCommonError);
            }
        }

        private static async Task<IResult> PutTeacher(HttpContext context, ITeacherService teacherService, int id, Teacher teacher)
        {
            try
            {
                return Results.Ok(await teacherService.Put(id,teacher));
            }
            catch (Exception)
            {
                return Results.BadRequest(Helper.ApiCommonError);
            }
        }

        private static async Task<IResult> PostTeacher(HttpContext context, ITeacherService teacherService, Teacher teacher)
        {
            try
            {
                return Results.Ok(await teacherService.Post(teacher));
            }
            catch (Exception)
            {
                return Results.BadRequest(Helper.ApiCommonError);
            }
        }

        private static async Task<IResult> GetAllTeacher(HttpContext context, ITeacherService teacherService)
        {
            try
            {
                return Results.Ok(await teacherService.Get());
            }
            catch (Exception)
            {
                return Results.BadRequest(Helper.ApiCommonError);
            }
        }

        private static async Task<IResult> GetTeacherById(HttpContext context, ITeacherService teacherService, int id)
        {
            try
            {
                return Results.Ok(await teacherService.GetById(id));
            }
            catch (Exception)
            {
                return Results.BadRequest(Helper.ApiCommonError);
            }
        }
    }
}
