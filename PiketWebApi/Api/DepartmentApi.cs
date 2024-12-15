using PiketWebApi.Abstractions;
using PiketWebApi.Data;
using PiketWebApi.Services;
using SharedModel.Models;

namespace PiketWebApi.Api
{
    public static class DepartmentApi
    {
        public static RouteGroupBuilder MapDepartmentApi(this RouteGroupBuilder group)
        {
            group.MapGet("/", GetAllDepartment);
            group.MapGet("/{id}", GetDepartmentById);
            group.MapPost("/", PostDepartment);
            group.MapPut("/{id}", PutDepartment);
            group.MapDelete("/{id}", DeleteDepartment);
            return group.WithTags("department").RequireAuthorization(); ;
        }

        private static async Task<IResult>  DeleteDepartment(HttpContext context, IDepartmentService departmenService, int id)
        {
            var result = await departmenService.DeleteAsync(id);
            return result.Match(items => Results.Ok(items), errors => Results.BadRequest(result.CreateProblemDetail(context)));
        }

        private static async Task<IResult> PutDepartment(HttpContext context, IDepartmentService departmenService, int id, Department model)
        {
            var result = await departmenService.PutAsync(id, model);
            return result.Match(items => Results.Ok(items), errors => Results.BadRequest(result.CreateProblemDetail(context)));
        }

        private static async Task<IResult> PostDepartment(HttpContext context, IDepartmentService departmenService, Department model)
        {
            var result = await departmenService.PostAsync(model);
            return result.Match(items => Results.Ok(items), errors => Results.BadRequest(result.CreateProblemDetail(context)));
        }

        private static async Task<IResult> GetAllDepartment(HttpContext context, IDepartmentService departmenService)
        {
            var result = await departmenService.GetAllAsync();
            return result.Match(items => Results.Ok(items), errors => Results.BadRequest(result.CreateProblemDetail(context)));
        }

        private static async Task<IResult> GetDepartmentById(HttpContext context, IDepartmentService departmenService, int id)
        {
            var result = await departmenService.GetByIdAsync(id);
            return result.Match(items => Results.Ok(items), errors => Results.BadRequest(result.CreateProblemDetail(context)));
        }
    }
}
