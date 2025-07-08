
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
        Task<ErrorOr<ClassRoomResponse>> CreateClassRoomFromLastClass(ClassRoomFromLastClassRequest req);
        Task<ErrorOr<bool>> DeleteClassRoom(int id);
        Task<ErrorOr<IEnumerable<ClassRoomResponse>>> GetAllClassRoom();
        Task<ErrorOr<ClassRoomResponse>> GetClassRoomById(int id);
        Task<ErrorOr<IEnumerable<ClassRoomResponse>>> GetClassRoomBySchoolYear(int id);
        Task<ErrorOr<IEnumerable<ClassRoomResponse>>> GetClassRoomByTeacherId(int id);
        Task<ErrorOr<ClassRoomResponse>> PostClassRoom(ClassRoomRequest req);
        Task<ErrorOr<bool>> PutClassRoom(int id, ClassRoomRequest req);
        Task<ErrorOr<bool>> RemoveStudentFromClassRoom(int classroomId, int studentId);
    }

    public class ClassRoomService : IClassRoomService
    {
        private IHttpContextAccessor http;
        private UserManager<ApplicationUser> userManager;
        private ApplicationDbContext dbContext;
        private readonly ISchoolYearService schoolYearService;

        public ClassRoomService(
            IHttpContextAccessor _http,
            UserManager<ApplicationUser> _userManager,
            ApplicationDbContext _dbContext,
            ISchoolYearService _schoolYearService
          )
        {
            http = _http;
            userManager = _userManager;
            dbContext = _dbContext;
            schoolYearService = _schoolYearService;

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
                var schoolYearActive = await schoolYearService.GetActiveSchoolYear();
                if (schoolYearActive.IsError)
                    return schoolYearActive.Errors;


                var result = dbContext.ClassRooms
                    .Include(x => x.SchoolYear)
                    .Include(x => x.Department)
                    .Include(x => x.HomeroomTeacher)
                    .Include(x => x.ClassLeader)
                    .Include(x => x.Students).ThenInclude(x => x.Student).Where(x => x.SchoolYear.Id == schoolYearActive.Value.Id)
                    .Select(x => new ClassRoomResponse(x.Id, x.Name, x.Level, x.SchoolYear.Id, x.SchoolYear.Year,
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
                  .Select(x => new ClassRoomResponse(x.Id, x.Name, x.Level, x.SchoolYear.Id, x.SchoolYear.Year,
                  x.Department.Id, x.Department.Name, x.Department.Initial, x.ClassLeader.Id,
                  x.ClassLeader.Name, x.HomeroomTeacher.Id, x.HomeroomTeacher.Name,
                  x.Students.Select(x => new
                  {
                      Id = x.Student.Id,
                      NIS = x.Student.NIS,
                      NISN = x.Student.NISN,
                      Name = x.Student.Name,
                      Gender = x.Student.Gender,
                      PlaceOfBorn = x.Student.PlaceOfBorn,
                      DateOfBorn = x.Student.DateOfBorn,
                  })));

                if (!result.Any())
                    return Error.Conflict("ClassRoom", "Kelas tidak ditemukan.");

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
                    .Select(x => new ClassRoomResponse(x.Id, x.Name, x.Level, x.SchoolYear.Id, x.SchoolYear.Year,
                    x.Department.Id, x.Department.Name, x.Department.Initial, x.ClassLeader.Id, x.ClassLeader.Name,
                    x.HomeroomTeacher.Id, x.HomeroomTeacher.Name, null));
                return await Task.FromResult(result.ToList());
            }
            catch (Exception)
            {
                return Error.Conflict();
            }
        }

        public async Task<ErrorOr<ClassRoomResponse>> PostClassRoom(ClassRoomRequest req)
        {
            try
            {

                var schoolYearActive = await schoolYearService.GetActiveSchoolYear();
                if (schoolYearActive.IsError)
                    return schoolYearActive.Errors;

                var validator = new ClassRoomValidator();
                var resultValidator = await validator.ValidateAsync(req);

                if (!resultValidator.IsValid)
                {
                    return resultValidator.GetErrors();
                }

                var includableQueryable = dbContext.ClassRooms
                                    .Include(x => x.Department)
                                    .Include(x => x.HomeroomTeacher)
                                    .Include(x => x.ClassLeader)
                                    .Include(x => x.SchoolYear).Where(x => x.SchoolYear.Id == schoolYearActive.Value.Id);
                //Check Teacher is HomeroomTeacer


                var classRoom = includableQueryable.FirstOrDefault(x => x.HomeroomTeacher.Id == req.HomeRoomTeacherId);
                if (classRoom != null)
                    return Error.Conflict("ClassRoom", $"{classRoom.HomeroomTeacher.Name} sudah menjadi wali kelas {classRoom.Name} {classRoom.Department.Name}");


                classRoom = includableQueryable.FirstOrDefault(x => x.ClassLeader.Id == req.ClassRommLeaderId);
                if (classRoom != null)
                    return Error.Conflict("ClassRoom", $"{classRoom.ClassLeader.Name} sudah menjadi ketua kelas {classRoom.Name} {classRoom.Department.Name}");


                var student = req.ClassRommLeaderId > 0 ? new Student { Id = req.ClassRommLeaderId } : null;
                var teacher = req.HomeRoomTeacherId > 0 ? new Teacher { Id = req.HomeRoomTeacherId } : null;
                var department = req.DepartmentId > 0 ? new Department { Id = req.DepartmentId } : null;

                var model = new ClassRoom()
                {
                    Name = req.Name,
                    Level = req.Level,
                    SchoolYear = schoolYearActive.Value,
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
                return await Task.FromResult(xx.Value);
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
                var validator = new ClassRoomValidator();
                var resultValidator = await validator.ValidateAsync(req);

                if (!resultValidator.IsValid)
                {
                    return resultValidator.GetErrors();
                }

                var result = dbContext.ClassRooms
                    .Include(x => x.Department)
                    .Include(x => x.HomeroomTeacher)
                    .Include(x => x.ClassLeader)
                    .SingleOrDefault(x => x.Id == id);

                if (result != null)
                {
                    result.Name = req.Name;
                    result.Level = req.Level;


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

        public async Task<ErrorOr<bool>> AddStudentToClassRoom(int classroomId, Student student)
        {
            try
            {

                var schoolYearActive = dbContext.SchoolYears.SingleOrDefault(x => x.Actived);
                if (schoolYearActive == null)
                    return Error.Failure("SchoolYear", "Tahun ajaran baru belum dibuka.");



                var classroom = dbContext.ClassRooms.Include(x => x.Students)
                    .ThenInclude(x => x.Student)
                    .SingleOrDefault(x => x.Id == classroomId);
                if (classroom == null)
                    return Error.Failure("Class Room", "Data Kelas Tidak Ditemukan");

                //student register on other class
                var studentExists = from x in dbContext.ClassRooms
                                .Include(x => x.Department)
                                .Include(x => x.Students)
                                .ThenInclude(x => x.Student)
                                .Where(x => x.SchoolYear.Id == schoolYearActive.Id &&
                                x.Students.Any(x => x.Student.Id == student.Id))
                                    select x;
                if (studentExists.Any())
                {
                    var s = studentExists.FirstOrDefault();
                    return Error.Failure("ClassRoom", $"{s.Students.Where(x => x.Student.Id == student.Id).FirstOrDefault().Student.Name} sudah terdaftar di kelas {s.Name}-{s.Department.Initial}");
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

        public async Task<ErrorOr<bool>> RemoveStudentFromClassRoom(int classroomId, int studentId)
        {
            try
            {

                var classroom = dbContext.ClassRooms
                    .Include(x => x.ClassLeader)
                    .Include(x => x.Students)
                    .ThenInclude(x => x.Student).SingleOrDefault(x => x.Id == classroomId);
                if (classroom == null)
                    return Error.Failure("ClassRoom", "Kelas tidak ditemukan");

                var member = classroom.Students.SingleOrDefault(x => x.Student.Id == studentId);
                if (member == null)
                    return Error.Failure("ClassRoom", "Data siswa tidak ditemukan");

                if (classroom?.ClassLeader?.Id == studentId)
                {
                    return Error.Failure("ClassRoom", $"{member.Student.Name} adalah Ketua Kelas. Pilih  Ketua Kelas baru sebelum mengeluarkannya.");
                }
                classroom?.Students.Remove(member);
                dbContext.SaveChanges();
                return await Task.FromResult(true);
            }
            catch (Exception)
            {
                return Error.Conflict();
            }
        }

        public async Task<ErrorOr<ClassRoomResponse>> CreateClassRoomFromLastClass(ClassRoomFromLastClassRequest req)
        {
            var schoolYearActive = await schoolYearService.GetActiveSchoolYear();
            if (schoolYearActive.IsError)
                return schoolYearActive.Errors;


            var includableQueryable = dbContext.ClassRooms
                                .Include(x => x.Department)
                                .Include(x => x.HomeroomTeacher)
                                .Include(x => x.ClassLeader)
                                .Include(x => x.SchoolYear)
                .Include(x => x.Students).ThenInclude(x => x.Student)
                                ;

            var classroom = includableQueryable.Where(x => x.Id == req.ClassRoomId)
                .Include(x => x.HomeroomTeacher)
                .Include(x => x.ClassLeader)
                .Include(x => x.Department)
                .FirstOrDefault();

            if (classroom == null)
                return Error.Failure("ClassRoom", "Data kelas tidak ditemukan");


            //Check Teacher is HomeroomTeacer

            var classRoom = includableQueryable.Where(x => x.SchoolYear.Id == schoolYearActive.Value.Id)
                .FirstOrDefault(x => x.HomeroomTeacher.Id == classroom.HomeroomTeacher.Id);
            if (classRoom != null)
                return Error.Conflict("ClassRoom", $"{classRoom.HomeroomTeacher.Name} sudah menjadi wali kelas {classRoom.Name} {classRoom.Department.Name}");


            classRoom = includableQueryable.Where(x => x.SchoolYear.Id == schoolYearActive.Value.Id)
                .FirstOrDefault(x => x.ClassLeader.Id == classroom.ClassLeader.Id);
            if (classRoom != null)
                return Error.Conflict("ClassRoom", $"{classRoom.ClassLeader.Name} sudah menjadi ketua kelas {classRoom.Name} {classRoom.Department.Name}");


            var newClassRoom = new ClassRoom()
            {
                ClassLeader = classroom.ClassLeader,
                Department = classroom.Department,
                HomeroomTeacher = classroom.HomeroomTeacher,
                Name = req.Name,
                Level = req.Level,
                SchoolYear = schoolYearActive.Value,
            };


            dbContext.Entry(newClassRoom.SchoolYear).State = EntityState.Unchanged;
            dbContext.Entry(newClassRoom.ClassLeader).State = EntityState.Unchanged;
            dbContext.Entry(newClassRoom.Department).State = EntityState.Unchanged;
            dbContext.Entry(newClassRoom.HomeroomTeacher).State = EntityState.Unchanged;
            foreach (var item in classroom.Students)
            {
                dbContext.Entry(item.Student).State = EntityState.Unchanged;
                newClassRoom.Students.Add(new ClassRoomMember { Student=item.Student });
            }

            dbContext.ClassRooms.Add(newClassRoom);
            dbContext.SaveChanges();

            return await GetClassRoomById(newClassRoom.Id);



        }

        public async Task<ErrorOr<IEnumerable<ClassRoomResponse>>> GetClassRoomByTeacherId(int id)
        {
            try
            {
                var result = dbContext.ClassRooms
                    .Include(x => x.SchoolYear)
                    .Include(x => x.Department)
                    .Include(x => x.HomeroomTeacher)
                    .Include(x => x.ClassLeader)
                    .Include(x => x.Students).ThenInclude(x => x.Student)
                    .Where(x => x.HomeroomTeacher.Id == id)
                    .Select(x => new ClassRoomResponse(x.Id, x.Name, x.Level, x.SchoolYear.Id, x.SchoolYear.Year,
                    x.Department.Id, x.Department.Name, x.Department.Initial, x.ClassLeader.Id, x.ClassLeader.Name,
                    x.HomeroomTeacher.Id, x.HomeroomTeacher.Name, null));
                return await Task.FromResult(result.ToList());
            }
            catch (Exception)
            {
                return Error.Conflict();
            }
        }
    }
}
