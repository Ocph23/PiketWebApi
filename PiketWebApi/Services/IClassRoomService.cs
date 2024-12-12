
using ErrorOr;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PiketWebApi.Data;
using PiketWebApi.Validators;
using SharedModel.Requests;
using SharedModel.Responses;

namespace PiketWebApi.Services
{
    public interface IClassRoomService
    {
        Task<ErrorOr<bool>> AddStudentToClassRoom(int classroomId, Student student);
        Task<ErrorOr<bool>> DeleteClassRoom(int id);
        Task<ErrorOr<IEnumerable<ClassRoomResponse>>> GetAllClassRoom();
        Task<ErrorOr<ClassRoomResponse>> GetClassRoomById(int id);
        Task<ErrorOr<IEnumerable<ClassRoomResponse>>> GetClassRoomBySchoolYear(int id);
        Task<ErrorOr<ErrorOr<ClassRoomResponse>>> PostClassRoom(ClassRoomRequest req);
        Task<ErrorOr<bool>> PutClassRoom(int id, ClassRoomRequest req);
        Task<ErrorOr<bool>> RemoveStudentFromClassRoom(int classroomId, int studentId);
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

        public async Task<ErrorOr<bool>> AddStudentToClassRoom(int classroomId, Student student)
        {
            try
            {

                var classroom = dbContext.ClassRooms.Include(x => x.Students).ThenInclude(x => x.Student).SingleOrDefault(x => x.Id == classroomId);
                if (classroom == null)
                    return Error.Failure("Class Room", "Data Kelas Tidak Ditemukan");

                var existsStudent = classroom.Students.SingleOrDefault(x => x.Student.Id == student.Id);
                if (existsStudent != null)
                {
                    return Error.Failure("Class Room", $"{student.Name} Sudah Terdaftar di kelas ini ");
                }
                var member = new ClassRoomMember { Student = student };
                dbContext.Entry(member.Student).State = EntityState.Unchanged;
                classroom.Students.Add(member);
                dbContext.SaveChanges();
                return await Task.FromResult(true);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<ErrorOr<bool>> DeleteClassRoom(int id)
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
                return await Task.FromResult(true);
            }
            catch (Exception)
            {
                return Error.Conflict();
            }
        }

        public async Task<ErrorOr<IEnumerable<ClassRoomResponse>>> GetAllClassRoom()
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

                return await Task.FromResult(result.ToList());
            }
            catch (Exception)
            {
                return Error.Conflict(); ;
            }
        }

        public async Task<ErrorOr<ClassRoomResponse>> GetClassRoomById(int id)
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
                  x.Students.Select(x => new { Id = x.Student.Id, NIS = x.Student.NIS, NISN= x.Student.NISN, Name = x.Student.Name })));
                return await Task.FromResult(result.FirstOrDefault());
            }
            catch (Exception)
            {
                return Error.Conflict(); ;
            }

        }

        public async Task<ErrorOr<IEnumerable<ClassRoomResponse>>> GetClassRoomBySchoolYear(int id)
        {
            try
            {
                var result = dbContext.ClassRooms
                    .Include(x => x.SchoolYear)
                    .Include(x => x.Department)
                    .Include(x => x.HomeroomTeacher)
                    .Include(x => x.ClassLeader)
                    .Include(x => x.Students).ThenInclude(x => x.Student)
                    .Where(x => x.SchoolYear.Id == id)
                    .Select(x => new ClassRoomResponse(x.Id, x.Name, x.SchoolYear.Id, x.SchoolYear.Year,
                    x.Department.Id, x.Department.Name, x.Department.Initial, x.ClassLeader.Id, x.ClassLeader.Name,
                    x.HomeroomTeacher.Id, x.HomeroomTeacher.Name, null));
                return await Task.FromResult(result.ToList());
            }
            catch (Exception)
            {
                return Error.Conflict();
            }
        }

        public async Task<ErrorOr<ErrorOr<ClassRoomResponse>>> PostClassRoom(ClassRoomRequest req)
        {
            try
            {
                var shoolYearActive = dbContext.SchoolYears.SingleOrDefault(x => x.Actived);
                if (shoolYearActive == null)
                    return Error.Failure(StatusCodes.Status400BadRequest.ToString(), "Tahun ajaran baru belum dibuka.");


                var validator = new ClassRoomValidator();
                var result = await validator.ValidateAsync(req);
                List<Error> errors = new List<Error>();
                if (!result.IsValid)
                {
                    foreach (var item in result.Errors)
                    {
                        errors.Add(Error.Validation(item.PropertyName, item.ErrorMessage));
                    }
                    return errors;
                }

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
                var teacher = req.HomeRoomTeacherId > 0 ? new Teacher { Id = req.HomeRoomTeacherId } : null;
                var department = req.DepartmentId > 0 ? new Department { Id = req.DepartmentId } : null;

                var model = new ClassRoom()
                {
                    Name = req.Name,
                    SchoolYear = shoolYearActive,
                    ClassLeader = student ?? student,
                    HomeroomTeacher = teacher ?? teacher,
                    Department = department ?? department,
                };

                if (model.Department != null)
                    dbContext.Entry(model.Department).State = EntityState.Unchanged;
                if (model.ClassLeader != null)
                {
                    dbContext.Entry(model.ClassLeader).State = EntityState.Unchanged;
                    model.Students.Add(new ClassRoomMember
                    {
                        Student = student
                    });
                }
                if (model.HomeroomTeacher != null)
                    dbContext.Entry(model.HomeroomTeacher).State = EntityState.Unchanged;

                dbContext.ClassRooms.Add(model);
                dbContext.SaveChanges();
                var xx = await GetClassRoomById(model.Id);
                return xx;
            }
            catch (Exception ex)
            {
                return Error.Conflict(ex.Message);
            }
        }

        public async Task<ErrorOr<bool>> PutClassRoom(int id, ClassRoomRequest req)
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
                    return await Task.FromResult(true);
                }
                return Error.Conflict("ClassRoom", "Kelas tidak ditemukan.");
            }
            catch (Exception)
            {
                return Error.Conflict();
            }
        }

        public async Task<ErrorOr<bool>> RemoveStudentFromClassRoom(int classroomId, int studentId)
        {
            try
            {
                var classroom = dbContext.ClassRooms.Include(x => x.Students)
       .ThenInclude(x => x.Student).SingleOrDefault(x => x.Id == classroomId);
                if (classroom == null)
                   return Error.Failure("ClassRoom","Kelas tidak ditemukan");

                var member = classroom.Students.SingleOrDefault(x => x.Student.Id == studentId);
                if (member == null)
                   return Error.Failure("ClassRoom", "Data siswa tidak ditemukan");

                classroom.Students.Remove(member);
                dbContext.SaveChanges();
                return await Task.FromResult(true);
            }
            catch (Exception)
            {
                return Error.Conflict();
            }
        }
    }
}
