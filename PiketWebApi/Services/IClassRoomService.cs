
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PiketWebApi.Data;
using SharedModel.Models;
using SharedModel.Requests;
using SharedModel.Responses;
using static System.Net.WebRequestMethods;

namespace PiketWebApi.Services
{
    public interface IClassRoomService
    {
        Task<bool> AddStudentToClassRoom(int classroomId, Student student);
        Task<bool> DeleteClassRoom(int id);
        Task<IEnumerable<ClassRoomResponse>> GetAllClassRoom();
        Task<ClassRoomResponse> GetClassRoomById(int id);
        Task<ClassRoomResponse> PostClassRoom(ClassRoomRequest req);
        Task<bool> PutClassRoom(int id, ClassRoomRequest req);
        Task<bool> RemoveStudentFromClassRoom(int classroomId, int studentId);
    }

    public class ClassRoomService : IClassRoomService
    {
        private IHttpContextAccessor http;
        private UserManager<ApplicationUser> userManager;
        private ApplicationDbContext dbContext;

        public ClassRoomService(IHttpContextAccessor _http, UserManager<ApplicationUser> _userManager, ApplicationDbContext _dbContext)
        {
            http = _http;
            userManager = _userManager;
            dbContext = _dbContext;
        }

        public Task<bool> AddStudentToClassRoom(int classroomId, Student student)
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
                return Task.FromResult(true);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public Task<bool> DeleteClassRoom(int id)
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
                return Task.FromResult(true);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public Task<IEnumerable<ClassRoomResponse>> GetAllClassRoom()
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
                    x.HomeroomTeacher.Id, x.HomeroomTeacher.Name, null));
                return Task.FromResult(result.AsEnumerable());
            }
            catch (Exception)
            {
                throw;
            }
        }

        public Task<ClassRoomResponse> GetClassRoomById(int id)
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
                return Task.FromResult(result.FirstOrDefault());
            }
            catch (Exception ex)
            {
                throw;
            }

        }

        public async Task<ClassRoomResponse> PostClassRoom(ClassRoomRequest req)
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
                model.Students.Add(new ClassRoomMember
                {
                    Student = student
                });
                dbContext.ClassRooms.Add(model);
                dbContext.SaveChanges();
                var xx = await GetClassRoomById(model.Id);
                return xx;

            }
            catch (Exception ex)
            {
                throw;
            }

        }

        public Task<bool> PutClassRoom(int id, ClassRoomRequest req)
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
                    return Task.FromResult(true);
                }
                throw new SystemException("Kelas tidak ditemukan.");
            }
            catch (Exception)
            {
                throw;
            }
        }

        public Task<bool> RemoveStudentFromClassRoom(int classroomId, int studentId)
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
                return Task.FromResult(true);
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
