using ErrorOr;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PiketWebApi.Data;
using SharedModel.Models;
using SharedModel.Requests;
using SharedModel.Responses;
using System.Net.Sockets;
using System.Security.Claims;

namespace PiketWebApi.Services
{
    public interface IPicketService
    {
        Task<ErrorOr<LateAndGoHomeEarlyResponse>> AddStudentToLateComeHomeSoEarly(StudentToLateAndEarlyRequest late);
        Task<ErrorOr<bool>> RemoveStudentToLateComeHomeSoEarly(int id);
        Task<ErrorOr<PicketResponse>> CreateNewPicket();
        Task<ErrorOr<PicketResponse>> GetPicketToday();
        Task<ErrorOr<bool>> UpdatePicket(int id, PicketRequest picket);
    }

    public class PicketService : IPicketService
    {
        private readonly IHttpContextAccessor http;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly ApplicationDbContext dbContext;
        private readonly IStudentService studentService;
        private static PicketResponse picketToday;

        public PicketService(IHttpContextAccessor _http, UserManager<ApplicationUser> _userManager,
            ApplicationDbContext _dbContext, IStudentService _studentService)
        {
            http = _http;
            userManager = _userManager;
            dbContext = _dbContext;
            studentService = _studentService;
        }

        public async Task<ErrorOr<PicketResponse>> CreateNewPicket()
        {
            try
            {
                DateOnly dateNow = DateOnly.FromDateTime(DateTime.Now);
                var userClaim = await http.IsTeacherPicket(userManager, dbContext);
                if (!userClaim.Item1)
                    return Error.Unauthorized("Picket", "Maaf, Anda tidak sedang piket/anda tidak memiliki akses !");

                var ppicketToday = dbContext.Picket.SingleOrDefault(x => x.Date == dateNow);
                if (ppicketToday == null)
                {
                    ppicketToday = Picket.Create(userClaim.Item2);
                    dbContext.Entry(ppicketToday.CreatedBy).State = EntityState.Unchanged;
                    dbContext.Picket.Add(ppicketToday);
                    dbContext.SaveChanges();
                }
                return await GetPicketToday();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<ErrorOr<PicketResponse>> GetPicketToday()
        {
            try
            {
                DateOnly date = DateOnly.FromDateTime(DateTime.Now);

                var ppicketToday = dbContext.Picket
                    .Include(x => x.CreatedBy)
                    .Include(x => x.TeacherAttendance).ThenInclude(x => x.Teacher)
                    .Include(x => x.LateAndComeHomeEarly).ThenInclude(x => x.Student)
                    .FirstOrDefault(x => x.Date == date);
                if (ppicketToday == null || date != ppicketToday.Date)
                {
                    return Error.Failure("Picket", "Piket Belum Di buka");
                }

                var response = await GeneratePicketResponse(ppicketToday);

                return picketToday = response.Value;
            }
            catch (Exception ex)
            {
                throw new SystemException(ex.Message);
            }
        }

        public async Task<ErrorOr<LateAndGoHomeEarlyResponse>> AddStudentToLateComeHomeSoEarly(StudentToLateAndEarlyRequest late)
        {
            try
            {
                var userClaim = await http.IsTeacherPicket(userManager, dbContext);
                if (!userClaim.Item1)
                    return Error.Unauthorized("Picket", "Maaf, Anda tidak sedang piket/anda tidak memiliki akses !");

                DateOnly date = DateOnly.FromDateTime(DateTime.Now);
                var picket = dbContext.Picket
                        .Include(x => x.LateAndComeHomeEarly)
                        .ThenInclude(x => x.Student)
                        .FirstOrDefault(x => x.Date == date);
                if (picket == null || DateOnly.FromDateTime(DateTime.Now) != picket.Date)
                {
                    return Error.Failure("Picket", "Piket Belum Di buka");
                }

                if (picket.LateAndComeHomeEarly.Any(x => x.Student.Id == late.StudentId && x.LateAndGoHomeEarlyStatus == late.LateAndGoHomeEarlyStatus))
                {
                    return Error.Failure("Picket", $"Siswa sudah di daftarkan !");
                }


                var student = dbContext.Students.Find(late.StudentId);
                if (student == null)
                {
                    return Error.Failure("Picket", "Data siswa tidak ditemukan");
                }


                var toLate = new LateAndGoHomeEarly
                {
                    Student = student,
                    CreatedBy = userClaim.Item2,
                    AttendanceStatus = late.AttendanceStatus,
                    LateAndGoHomeEarlyStatus = late.LateAndGoHomeEarlyStatus,
                    CreateAt = DateTime.Now.ToUniversalTime(),
                    Description = late.Description,
                    Time = late.AtTime
                };

                picket.LateAndComeHomeEarly.Add(toLate);
                dbContext.SaveChanges();
                var result = await GenerateLateAndGoHomeEarlyResponse(toLate);
                return result;
            }
            catch (Exception ex)
            {
                return Error.Conflict();
            }
        }
        public async Task<ErrorOr<bool>> RemoveStudentToLateComeHomeSoEarly(int id)
        {
            try
            {
                var userClaim = await http.IsTeacherPicket(userManager, dbContext);
                if (!userClaim.Item1)
                    return Error.Unauthorized("Picket", "Maaf, Anda tidak sedang piket/anda tidak memiliki akses !");

                var picket = dbContext.Picket
                      .Include(x => x.LateAndComeHomeEarly).Where(x => x.LateAndComeHomeEarly.Any(x => x.Id == id)).FirstOrDefault();

                var result = picket.LateAndComeHomeEarly.Where(x => x.Id == id).FirstOrDefault();
                if (result == null)
                    return Error.NotFound("Picket", "Data tidak ditemukan.");
                picket.LateAndComeHomeEarly.Remove(result);
                dbContext.SaveChanges();
                return await Task.FromResult(true);
            }
            catch (Exception ex)
            {
                return Error.Conflict();
            }
        }

        private async Task<LateAndGoHomeEarlyResponse?> GenerateLateAndGoHomeEarlyResponse(LateAndGoHomeEarly x)
        {
            var studentClass = await studentService.GetStudentWithClass(x.Id);
            var sx = studentClass.Value;
            return new LateAndGoHomeEarlyResponse
            {
                Id = x.Id,
                LateAndGoHomeEarlyStatus = x.LateAndGoHomeEarlyStatus,
                AttendanceStatus = x.AttendanceStatus,
                CreateAt = x.CreateAt,
                Time = x.Time,
                TeacherId = x.CreatedBy.Id,
                TeacherName = x.CreatedBy.Name,
                TeacherPhoto = x.CreatedBy.Photo,
                Description = x.Description,
                StudentPhoto = x.Student.Photo,
                StudentId = x.Student.Id,
                StudentName = x.Student.Name,
                ClassRoomId = sx == null ? null : sx.ClassRoomId,
                ClassRoomName = sx == null ? null : sx.ClassRoomName,
                DepartmentId = sx == null ? null : sx.DepartmenId,
                DepartmentName = sx == null ? null : sx.DepartmenName
            };
        }

        private async Task<ErrorOr<PicketResponse>> GeneratePicketResponse(Picket response)
        {
            var result = new PicketResponse()
            {
                CreateAt = response.CreateAt,
                CreatedId = response.CreatedBy.Id,
                CreatedName = response.CreatedBy.Name,
                CreatedNumber = response.CreatedBy.RegisterNumber,
                Date = response.Date,
                EndAt = response.EndAt,
                Id = response.Id,
                StartAt = response.StartAt,
                Weather = response.Weather,
                StudentsLateAndComeHomeEarly = Enumerable.Empty<LateAndGoHomeEarlyResponse>().ToList()
            };

            var students = await studentService.GetAlStudentWithClass();

            result.StudentsLateAndComeHomeEarly = (from x in response.LateAndComeHomeEarly
                                                   join s in students.Value on x.Student.Id equals s.Id into sGroup
                                                   from sx in sGroup.DefaultIfEmpty()
                                                   select
                                                   new LateAndGoHomeEarlyResponse
                                                   {
                                                       Id = x.Id,
                                                       LateAndGoHomeEarlyStatus = x.LateAndGoHomeEarlyStatus,
                                                       AttendanceStatus = x.AttendanceStatus,
                                                       Time = x.Time,
                                                       CreateAt = x.CreateAt,
                                                       TeacherId = x.CreatedBy.Id,
                                                       TeacherName = x.CreatedBy.Name,
                                                       TeacherPhoto = x.CreatedBy.Photo,
                                                       Description = x.Description,
                                                       StudentPhoto = x.Student.Photo,
                                                       StudentId = x.Student.Id,
                                                       StudentName = x.Student.Name,
                                                       ClassRoomId = sx == null ? null : sx.ClassRoomId,
                                                       ClassRoomName = sx == null ? null : sx.ClassRoomName,
                                                       DepartmentId = sx == null ? null : sx.DepartmenId,
                                                       DepartmentName = sx == null ? null : sx.DepartmenName

                                                   }).ToList();



            return result;

        }

        public async Task<ErrorOr<bool>> UpdatePicket(int id, PicketRequest model)
        {
            try
            {
                var result = dbContext.Picket.Include(x => x.CreatedBy)
                    .SingleOrDefault(x => x.Id == id);

                if (result == null)
                    return Error.NotFound("Picket", "Data piket tidak ditemukan.");

                result.Weather = model.Weather;
                result.StartAt = model.StartAt;
                result.EndAt = model.EndAt;
                if (result.CreatedBy.Id != model.CreatedId)
                {
                    result.CreatedBy = new Teacher { Id = model.Id };
                    dbContext.Entry(result.CreatedBy).State = EntityState.Unchanged;
                }
                dbContext.SaveChanges();
                return await Task.FromResult(true);
            }
            catch (Exception ex)
            {
                return Error.Conflict("Picket", ex.Message);
            }
        }
    }
}
