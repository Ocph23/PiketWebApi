
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
    public static class StudentApi
    {
        public static RouteGroupBuilder MapStudentApi(this RouteGroupBuilder group)
        {
            group.MapGet("/", GetAllStudent);
            group.MapPost("/paginate", GetAllStudentWithPanitate);
            group.MapGet("/withclass", GetAllStudentWithClass);
            group.MapGet("/withclas/{id}", GetStudentWithClass);
            group.MapGet("/{id}", GetStudentById);
            group.MapGet("/search/{searchtext}", SearchStudent);
            group.MapPost("/", PostStudent);
            group.MapPut("/{id}", PutStudent);
            group.MapDelete("/{id}", DeleteStudent);
            group.MapPut("/photo/{id}", UploadFoto);
            group.MapGet("/photo/{fileName}", GetFoto).AllowAnonymous();
            return group.WithTags("student").RequireAuthorization(); ;
        }

        private static async Task<IResult> GetFoto(HttpContext context, string fileName)
        {
            // Set the directory where the files are stored
            string fileDirectory = Path.Combine(Directory.GetCurrentDirectory(), "photos/student");

            // Ensure the directory exists
            if (!Directory.Exists(fileDirectory))
            {
                return Results.BadRequest("File directory does not exist.");
            }

            // Build the full file path
            string filePath = Path.Combine(fileDirectory, fileName);

            // Check if the file exists
            if (!System.IO.File.Exists(filePath))
            {
                return Results.BadRequest("File not found.");
            }

            // Read the file content
            var fileBytes = await System.IO.File.ReadAllBytesAsync(filePath);

            // Return the file as a download
           return Results.File(fileBytes, "application/octet-stream", fileName);
        }

        private static async Task<IResult> GetAllStudentWithPanitate(HttpContext context, IStudentService studentService, PaginationRequest req)
            
        {
            var result = await studentService.GetAllStudentWithPanitate(req);
            return result.Match(items => Results.Ok(items), errors => Results.BadRequest(result.CreateProblemDetail(context)));

        }

        private static async Task<IResult> UploadFoto(HttpContext context, IStudentService studentService, int id, byte[] data)
        {
            var result = await studentService.UploadPhoto(id, data);
            return result.Match(items => Results.Ok(items), errors => Results.BadRequest(result.CreateProblemDetail(context)));

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
