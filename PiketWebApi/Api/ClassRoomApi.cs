
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PiketWebApi.Data;
using PiketWebApi.Models;
using SharedModel.Models;
using SharedModel.Requests;
using SharedModel.Responses;
using System.Linq;

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

        private static async Task<IResult> GetClassRoomById(HttpContext context, ApplicationDbContext dbContext, int id)
        {
            try
            {
                var result = dbContext.ClassRooms
                    .Include(x => x.SchoolYear)
                    .Include(x => x.Department)
                    .Include(x => x.HomeroomTeacher)
                    .Include(x => x.ClassLeader)
                    .Include(x => x.Students).ThenInclude(x => x.Student)
                    .Where(x => x.Id == id)
                    .Select(x => new ClassRoomResponse(x.Id, x.Name, x.SchoolYear.Id, x.SchoolYear.Year,
                    x.Department.Id, x.Department.Name, x.Department.Initial, x.ClassLeader.Id, x.ClassLeader.Name,
                    x.HomeroomTeacher.Id, x.HomeroomTeacher.Name, 
                    x.Students.Select(x => new { Id = x.Student.Id, Nis = x.Student.Number, Name = x.Student.Name })));
                return Results.Ok(result.SingleOrDefault());
            }
            catch (Exception ex)
            {
                return Results.BadRequest(ex.Message);
            }
        }

        private static IResult RemoveStudentFromClassRoom(HttpContext context, ApplicationDbContext dbContext, int classroomId, int studentId)
        {
            try
            {
                var classroom = dbContext.ClassRooms.Include(x => x.Students)
                    .ThenInclude(x => x.Student).SingleOrDefault(x => x.Id == classroomId);
                if (classroom == null)
                    throw new SystemException("Class Room Not Found");

                var member = classroom.Students.SingleOrDefault(x => x.Student.Id == studentId);
                if (member == null)
                    throw new SystemException("Student  Not Found");

                classroom.Students.Remove(member);
                dbContext.SaveChanges();
                return TypedResults.Ok();
            }
            catch (Exception ex)
            {
                return TypedResults.BadRequest(ex.Message);
            }
        }

        private static IResult AddStudentToClassRoom(HttpContext context, ApplicationDbContext dbContext, int classroomId, Student student)
        {
            try
            {

                var classroom = dbContext.ClassRooms.Include(x => x.Students).ThenInclude(x => x.Student).SingleOrDefault(x => x.Id == classroomId);
                if (classroom == null)
                    throw new SystemException("Class Room Not Found");

                var existsStudent = classroom.Students.SingleOrDefault(x => x.Student.Id == student.Id);
                if (existsStudent != null)
                {
                    throw new SystemException($"{student.Name} Sudah Terdaftar di kelas ini ");
                }

                var member = new ClassRoomMember { Student = student };
                dbContext.Entry(member.Student).State = EntityState.Unchanged;
                classroom.Students.Add(member);
                dbContext.SaveChanges();
                return TypedResults.Ok(true);
            }
            catch (Exception ex)
            {
                return TypedResults.BadRequest(ex.Message);
            }
        }

        private static IResult DeleteClassRoom(HttpContext context, ApplicationDbContext dbContext, int id)
        {
            try
            {
                var result = dbContext.ClassRooms
                           .Include(x => x.SchoolYear)
                    .Include(x => x.Department)
                    .Include(x => x.ClassLeader)
                    .Include(x => x.HomeroomTeacher)
                    .Include(x => x.Students)


                    .SingleOrDefault(x => x.Id == id);
                if (result != null)
                {
                    dbContext.Remove(result);
                    dbContext.SaveChanges();
                }
                return Results.Ok(true);
            }
            catch (Exception ex)
            {
                return Results.BadRequest(ex.Message);
            }
        }

        private static IResult PutClassRoom(HttpContext context, ApplicationDbContext dbContext, int id, ClassRoomRequest req)
        {
            try
            {
                var result = dbContext.ClassRooms
                    .Include(x => x.Department)
                    .Include(x => x.HomeroomTeacher)
                    .Include(x => x.ClassLeader)
                    .SingleOrDefault(x => x.Id == id);
                if (result != null)
                {
                    result.Name = req.Name;


                    if (result.ClassLeader.Id != req.ClassRommLeaderId)
                    {
                        result.ClassLeader = new Student { Id = req.ClassRommLeaderId };
                        dbContext.Entry(result.ClassLeader).State = EntityState.Unchanged;
                    }

                    if (result.Department.Id != req.DepartmentId)
                    {
                        result.Department = new Department { Id = req.DepartmentId };
                        dbContext.Entry(result.Department).State = EntityState.Unchanged;
                    }


                    if (result.HomeroomTeacher.Id != req.HomeRoomTeacherId)
                    {
                        result.HomeroomTeacher = new Teacher { Id = req.HomeRoomTeacherId };
                        dbContext.Entry(result.HomeroomTeacher).State = EntityState.Unchanged;

                    }

                    dbContext.SaveChanges();
                    return Results.Ok(true);
                }
                throw new SystemException("Kelas tidak ditemukan.");
            }
            catch (Exception ex)
            {
                return Results.BadRequest(ex.Message);
            }
        }

        private static IResult PostClassRoom(HttpContext context, ApplicationDbContext dbContext, ClassRoomRequest req)
        {
            try
            {

                var shoolYearActive = dbContext.SchoolYears.SingleOrDefault(x => x.Actived);
                if (shoolYearActive == null)
                    throw new SystemException("Tahun Ajaran belum ada, hubungi administrator");

                Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<ClassRoom, SchoolYear> includableQueryable = dbContext.ClassRooms
                                    .Include(x => x.Department)
                                    .Include(x => x.HomeroomTeacher)
                                    .Include(x => x.SchoolYear);
                ClassRoom classRoom = includableQueryable
                    .FirstOrDefault(x => x.HomeroomTeacher != null
                    && x.HomeroomTeacher.Id == req.HomeRoomTeacherId
                    && x.SchoolYear.Id == shoolYearActive.Id);

                if (classRoom != null)
                    throw new SystemException($"{classRoom.HomeroomTeacher.Name} sudah menjadi wali kelas {classRoom.Name} {classRoom.Department.Name}");


                var student = req.ClassRommLeaderId > 0 ? new Student { Id = req.ClassRommLeaderId } : null;
                var model = new ClassRoom()
                {
                    Name = req.Name,
                    SchoolYear = shoolYearActive,
                    ClassLeader = student,
                    HomeroomTeacher = req.HomeRoomTeacherId > 0 ? new Teacher { Id = req.HomeRoomTeacherId } : null,
                    Department = req.DepartmentId > 0 ? new Department { Id = req.DepartmentId } : null,
                };

                dbContext.Entry(model.Department).State = EntityState.Unchanged;
                dbContext.Entry(model.HomeroomTeacher).State = EntityState.Unchanged;
                dbContext.Entry(model.ClassLeader).State = EntityState.Unchanged;
                model.Students.Add(new ClassRoomMember { Student = student });
                var result = dbContext.ClassRooms.Add(model);
                dbContext.SaveChanges();
                var xx = new ClassRoomResponse(model.Id, model.Name, model.SchoolYear.Id, model.SchoolYear.Year,
                    model.Department.Id, model.Department.Name, model.Department.Initial, model.ClassLeader.Id, model.ClassLeader.Name,
                    model.HomeroomTeacher.Id, model.HomeroomTeacher.Name, null);
                return Results.Ok(model);
            }
            catch (Exception ex)
            {
                return Results.BadRequest(ex.Message);
            }
        }

        private static object GetAllClassRoom(HttpContext context, ApplicationDbContext dbContext)
        {
            try
            {
                var result = dbContext.ClassRooms
                    .Include(x => x.SchoolYear)
                    .Include(x => x.Department)
                    .Include(x => x.HomeroomTeacher)
                    .Include(x => x.ClassLeader)
                    .Include(x => x.Students).ThenInclude(x => x.Student)
                    .Select(x => new ClassRoomResponse(x.Id, x.Name, x.SchoolYear.Id, x.SchoolYear.Year,
                    x.Department.Id, x.Department.Name, x.Department.Initial, x.ClassLeader.Id, x.ClassLeader.Name,
                    x.HomeroomTeacher.Id, x.HomeroomTeacher.Name,null));
                return Results.Ok(result.ToList());
            }
            catch (Exception ex)
            {
                return Results.BadRequest(ex.Message);
            }
        }
    }
}
