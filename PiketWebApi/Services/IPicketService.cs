using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PiketWebApi.Data;
using SharedModel.Models;
using SharedModel.Requests;
using SharedModel.Responses;
using System.Net.Sockets;
using System.Security.Claims;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace PiketWebApi.Services
{
    public interface IPicketService
    {
        Task<StudentToLateAndComeHomeSoEarlyResponse> AddStudentToLateComeHomeSoEarly(StudentToLateAndEarlyRequest late);
        Task<PicketResponse> CreateNewPicket();
        Task<PicketResponse> GetPicketToday();
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

        public async Task<PicketResponse> CreateNewPicket()
        {
            try
            {
                DateOnly dateNow = DateOnly.FromDateTime(DateTime.Now);
                var userClaim = await http.IsTeacherPicket(userManager, dbContext);
                if (!userClaim.Item1)
                    throw new UnauthorizedAccessException("Maaf, Anda tidak sedang piket/anda tidak memiliki akses !");

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

        public async Task<PicketResponse> GetPicketToday()
        {
            try
            {
                DateOnly date = DateOnly.FromDateTime(DateTime.Now);
                if (picketToday != null && date == picketToday.Date)
                    return picketToday;

                if (picketToday != null && date != picketToday.Date)
                {
                    throw new SystemException("Piket Belum Di buka");
                }

                var ppicketToday = dbContext.Picket
                    .Include(x => x.CreatedBy)
                    .Include(x => x.TeacherAttendance).ThenInclude(x=>x.Teacher)
                    .Include(x => x.LateAndComeHomeEarly).ThenInclude(x => x.Student)
                    .SingleOrDefault(x => x.Date == date);
                if (ppicketToday == null || date != ppicketToday.Date)
                {
                    throw new SystemException("Piket Belum Di buka");
                }

                var response = await GeneratePicketResponse(ppicketToday);

                return picketToday = response;
            }
            catch (Exception ex)
            {
                throw new SystemException(ex.Message);
            }
        }



        public async Task<StudentToLateAndComeHomeSoEarlyResponse> AddStudentToLateComeHomeSoEarly(StudentToLateAndEarlyRequest late)
        {
            try
            {
                var userClaim = await http.IsTeacherPicket(userManager, dbContext);
                if (!userClaim.Item1)
                    throw new UnauthorizedAccessException("Maaf, Anda tidak sedang piket/anda tidak memiliki akses !");

                DateOnly date = DateOnly.FromDateTime(DateTime.Now);
                var picket = dbContext.Picket
                        .Include(x => x.LateAndComeHomeEarly).ThenInclude(x => x.Student)
                        .SingleOrDefault(x => x.Date == date);
                if (picket == null || DateOnly.FromDateTime(DateTime.Now) != picket.Date)
                {
                    throw new SystemException("Piket Belum Di buka");
                }

                if (picket.LateAndComeHomeEarly.Any(x => x.Student.Id == late.StudentId))
                {
                    throw new SystemException("Siswa sudah di daftarkan !");
                }

                var toLate = new 
                {
                    Student = new Student { Id = late.StudentId },
                    CreatedBy = userClaim.Item2,
                    AttendanceStatus = SharedModel.AttendanceStatus.Hadir,
                    CreateAt = DateTime.Now.ToUniversalTime(),
                    Description = late.Description,
                    AtTime = late.AtTime
                };

                dbContext.Entry(toLate.Student).State = EntityState.Unchanged;
                picket.LateAndComeHomeEarly.Add(toLate);
                dbContext.SaveChanges();
                var result = GenerateStudentToLateAndComeHomeSoEarlyResponse(toLate);

                picketToday.StudentsToLate.Add(result);

                return result;
            }
            catch (Exception ex)
            {
                throw;
            }
        }



        private async Task<StudentToLateAndComeHomeSoEarlyResponse?> GenerateStudentToLateAndComeHomeSoEarlyResponse(StudentToLate x)
        {
            var sx = await studentService.GetStudentWithClass(x.Id);
            return new StudentToLateAndComeHomeSoEarlyResponse
            {
                Id = x.Id,
                AttendanceStatus = x.AttendanceStatus,
                CreateAt = x.CreateAt,
                AtTime = x.AtTime,
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

        private async Task<PicketResponse> GeneratePicketResponse(Picket response)
        {
            var result = new PicketResponse()
            {
                CreateAt = response.CreateAt,
                CreatedId = response.CreatedBy.Id,
                CreatedName = response.CreatedBy.Name,
                CreatedNumber = response.CreatedBy.Number,
                Date = response.Date,
                EndAt = response.EndAt,
                Id = response.Id,
                StartAt = response.StartAt,
                Weather = response.Weather,
                StudentsComeHomeEarly = Enumerable.Empty<StudentToLateAndComeHomeSoEarlyResponse>(),
                StudentsToLate = Enumerable.Empty<StudentToLateAndComeHomeSoEarlyResponse>()
            };

            var students = await studentService.GetAlStudentWithClass();
            result.StudentsToLate = (from x in response.StudentsToLate
                                     join s in students on x.Student.Id equals s.Id into sGroup
                                     from sx in sGroup.DefaultIfEmpty()
                                     select
                                     new StudentToLateAndComeHomeSoEarlyResponse
                                     {
                                         Id = x.Id,
                                         AttendanceStatus = x.AttendanceStatus,
                                         AtTime = x.AtTime,
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

                                     }).AsEnumerable();


            result.StudentsComeHomeEarly = (from x in response.StudentsComeHomeEarly
                                            join s in students on x.Student.Id equals s.Id into sGroup
                                            from sx in sGroup.DefaultIfEmpty()
                                            select new StudentToLateAndComeHomeSoEarlyResponse
                                            {
                                                Id = x.Id,
                                                AttendanceStatus = x.AttendanceStatus,
                                                CreateAt = x.CreateAt,
                                                AtTime = x.Time,
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
                                            }).AsEnumerable();


            return result;

        }
    }
}
