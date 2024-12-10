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
        Task<StudentComeHomeEarly> AddStudentComeHomeSoEarly(StudentToLateAndEarlyRequest early);
        Task<StudentToLate> AddStudentToLate(StudentToLateAndEarlyRequest late);
        Task<Picket> CreateNewPicket();
        Task<PicketResponse> GetPicketToday();
    }

    public class PicketService : IPicketService
    {
        private readonly IHttpContextAccessor http;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly ApplicationDbContext dbContext;
        private readonly IStudentService studentService;
        private static Picket picketToday;

        public PicketService(IHttpContextAccessor _http, UserManager<ApplicationUser> _userManager,
            ApplicationDbContext _dbContext, IStudentService _studentService)
        {
            http = _http;
            userManager = _userManager;
            dbContext = _dbContext;
            studentService = _studentService;
        }

        public async Task<Picket> CreateNewPicket()
        {
            try
            {
                var userClaim = await http.IsTeacherPicket(userManager, dbContext);
                if (!userClaim.Item1)
                    throw new UnauthorizedAccessException("Maaf, Anda tidak sedang piket/anda tidak memiliki akses !");

                if (picketToday == null || DateOnly.FromDateTime(DateTime.Now) != picketToday.Date)
                {
                    DateOnly date = DateOnly.FromDateTime(DateTime.Now);
                    picketToday = dbContext.Picket.FirstOrDefault(x => x.Date == date);
                    if (picketToday != null)
                        return picketToday;

                    picketToday = Picket.Create(userClaim.Item2);
                    dbContext.Entry(picketToday.CreatedBy).State = EntityState.Unchanged;
                    dbContext.Picket.Add(picketToday);
                    dbContext.SaveChanges();
                    return picketToday;
                }
                else
                {
                    return picketToday;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<PicketResponse> GetPicketToday()
        {
            try
            {
                if (picketToday == null)
                {
                    DateOnly date = DateOnly.FromDateTime(DateTime.Now);
                    picketToday = dbContext.Picket
                        .Include(x => x.CreatedBy)
                        .Include(x => x.TeacherAttendance)
                        .Include(x => x.StudentsToLate).ThenInclude(x => x.Student)
                        .Include(x => x.StudentsToLate).ThenInclude(x => x.CreatedBy)
                        .Include(x => x.StudentsComeHomeEarly).ThenInclude(x => x.Student)
                        .Include(x => x.StudentsComeHomeEarly).ThenInclude(x => x.CreatedBy)
                        .SingleOrDefault(x => x.Date == date);
                    if (picketToday == null || DateOnly.FromDateTime(DateTime.Now) != picketToday.Date)
                    {
                        throw new SystemException("Piket Belum Di buka");
                    }
                }
                if (picketToday != null && DateOnly.FromDateTime(DateTime.Now) != picketToday.Date)
                {
                    throw new SystemException("Piket Belum Di buka");
                }

                var response = new PicketResponse()
                {
                    CreateAt = picketToday.CreateAt,
                    CreatedId = picketToday.CreatedBy.Id,
                    CreatedName = picketToday.CreatedBy.Name,
                    CreatedNumber = picketToday.CreatedBy.Number,
                    Date = picketToday.Date,
                    EndAt = picketToday.EndAt,
                    Id = picketToday.Id,
                    StartAt = picketToday.StartAt,
                    Weather = picketToday.Weather
                };


                var students = await studentService.GetAlStudentWithClass();
                response.StudentsToLate = (from x in picketToday.StudentsToLate
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
                                               ClassRoomId = sx==null?null:sx.ClassRoomId,
                                               ClassRoomName = sx==null?null: sx.ClassRoomName,
                                               DepartmentId = sx == null ? null : sx.DepartmenId,
                                               DepartmentName= sx == null ? null : sx.DepartmenName

                                           }).AsEnumerable();


                response.StudentsComeHomeEarly = (from x in picketToday.StudentsComeHomeEarly
                                           join s in await studentService.GetAlStudentWithClass() on x.Student.Id equals s.Id into sGroup
                                           from sx in sGroup.DefaultIfEmpty()
                                           select
                                           new StudentToLateAndComeHomeSoEarlyResponse
                                           {
                                               Id = x.Id,
                                               AttendanceStatus = x.AttendanceStatus,
                                               AtTime = x.Time,
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





                return response;
            }
            catch (Exception ex)
            {
                throw new SystemException(ex.Message);
            }
        }

        public async Task<StudentToLate> AddStudentToLate(StudentToLateAndEarlyRequest late)
        {
            try
            {
                var userClaim = await http.IsTeacherPicket(userManager, dbContext);
                if (!userClaim.Item1)
                    throw new UnauthorizedAccessException("Maaf, Anda tidak sedang piket/anda tidak memiliki akses !");

                DateOnly date = DateOnly.FromDateTime(DateTime.Now);
                var picketToday = dbContext.Picket
                        .Include(x => x.StudentsToLate).ThenInclude(x => x.Student)
                        .SingleOrDefault(x => x.Date == date);
                if (picketToday == null || DateOnly.FromDateTime(DateTime.Now) != picketToday.Date)
                {
                    throw new SystemException("Piket Belum Di buka");
                }

                if (picketToday.StudentsToLate.Any(x => x.Student.Id == late.StudentId))
                {
                    throw new SystemException("Siswa sudah di daftarkan !");
                }

                var toLate = new StudentToLate
                {
                    Student = new Student { Id = late.StudentId },
                    CreatedBy = userClaim.Item2,
                    AttendanceStatus = SharedModel.StudentAttendanceStatus.Present,
                    CreateAt = DateTime.Now.ToUniversalTime(),
                    Description = late.Description,
                    AtTime = late.AtTime
                };

                dbContext.Entry(toLate.Student).State = EntityState.Unchanged;
                picketToday.StudentsToLate.Add(toLate);
                dbContext.SaveChanges();
                return toLate;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public async Task<StudentComeHomeEarly> AddStudentComeHomeSoEarly(StudentToLateAndEarlyRequest model)
        {
            try
            {
                var userClaim = await http.IsTeacherPicket(userManager, dbContext);
                if (!userClaim.Item1)
                    throw new UnauthorizedAccessException("Maaf, Anda tidak sedang piket/anda tidak memiliki akses !");

                DateOnly date = DateOnly.FromDateTime(DateTime.Now);
                var picketToday = dbContext.Picket
                        .Include(x => x.StudentsComeHomeEarly).ThenInclude(x => x.Student)
                        .SingleOrDefault(x => x.Date == date);
                if (picketToday == null || DateOnly.FromDateTime(DateTime.Now) != picketToday.Date)
                {
                    throw new SystemException("Piket Belum Di buka");
                }

                if (picketToday.StudentsComeHomeEarly.Any(x => x.Student.Id == model.StudentId))
                {
                    throw new SystemException("Siswa sudah di daftarkan !");
                }

                var toEarly= new StudentComeHomeEarly
                {
                    Student = new Student { Id = model.StudentId },
                    CreatedBy = userClaim.Item2,
                    AttendanceStatus = model.StudentAttendance,
                    CreateAt = DateTime.Now.ToUniversalTime(),
                    Description = model.Description,   
                    Time = model.AtTime
                };

                dbContext.Entry(toEarly.Student).State = EntityState.Unchanged;
                picketToday.StudentsComeHomeEarly.Add(toEarly);
                dbContext.SaveChanges();
                return toEarly;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
