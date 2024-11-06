using PiketWebApi.Data;
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

        private static IResult DeleteDepartment(HttpContext context, ApplicationDbContext dbContext, int id)
        {
            try
            {
                var result = dbContext.Departments.SingleOrDefault(x => x.Id == id);
                if (result != null)
                {
                    dbContext.Remove(result);
                    dbContext.SaveChanges();
                    return Results.Ok(true);
                }
                throw new Exception("Data Not Found !");
            }
            catch (Exception ex)
            {
                return Results.BadRequest(ex.Message);
            }
        }

        private static IResult PutDepartment(HttpContext context, ApplicationDbContext dbContext, int id, Department model)
        {
            try
            {
                var result = dbContext.Departments.SingleOrDefault(x=>x.Id==id);
                if (result != null)
                {
                    dbContext.Entry(result).CurrentValues.SetValues(model);
                    dbContext.SaveChanges();
                    return Results.Ok(true);
                }
                throw new Exception("Data Not Found !");

            }
            catch (Exception ex)
            {
                return Results.BadRequest(ex.Message);
            }
        }

        private static IResult PostDepartment(HttpContext context, ApplicationDbContext dbContext, Department model)
        {
            try
            {
                var result = dbContext.Departments.Add(model);
                dbContext.SaveChanges();
                return Results.Ok(model);
            }
            catch (Exception ex)
            {
                return Results.BadRequest(ex.Message);
            }
        }

        private static object GetAllDepartment(HttpContext context, ApplicationDbContext dbContext)
        {
            try
            {
                var result = dbContext.Departments.ToList();
                return Results.Ok(result);
            }
            catch (Exception ex)
            {
                return Results.BadRequest(ex.Message);
            }
        }

        private static IResult GetDepartmentById(HttpContext context, ApplicationDbContext dbContext, int id)
        {
            try
            {
                var result = dbContext.Departments.SingleOrDefault(x=>x.Id ==id);
                return Results.Ok(result);
            }
            catch (Exception ex)
            {
                return Results.BadRequest(ex.Message);
            }
        }
    }
}
