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
        private static Picket picketToday;

        public PicketService(IHttpContextAccessor _http, UserManager<ApplicationUser> _userManager,
            ApplicationDbContext _dbContext)
        {
            http = _http;
            userManager = _userManager;
            dbContext = _dbContext;
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

        public Task<PicketResponse> GetPicketToday()
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



                response.StudentsToLate = (from x in picketToday.StudentsToLate
                                           select
                                           new StudentToLateAndComeHomeSoEarlyResponse
                                           {
                                               AttendanceStatus = x.AttendanceStatus,
                                               AtTime = x.AtTime,
                                               CreateAt = x.CreateAt,
                                               TeacherId = x.CreatedBy.Id,
                                               TeacherName= x.CreatedBy.Name,
                                               TeacherPhoto = x.CreatedBy.Photo,
                                               Description = x.Description,
                                               Id = x.Id,
                                               StudentPhoto = x.Student.Photo,
                                               StudentId = x.Student.Id,
                                               StudentName = x.Student.Name
                                           }).AsEnumerable();


                response.StudentsComeHomeEarly = (from x in picketToday.StudentsComeHomeEarly
                                           select
                                           new StudentToLateAndComeHomeSoEarlyResponse
                                           {
                                               AtTime = x.Time,
                                               AttendanceStatus = x.AttendanceStatus,
                                               CreateAt = x.CreateAt,
                                               TeacherId = x.CreatedBy.Id,
                                               TeacherName= x.CreatedBy.Name,
                                               TeacherPhoto = x.CreatedBy.Photo,
                                               Description = x.Description,
                                               Id = x.Id,
                                               StudentPhoto = x.Student.Photo,
                                               StudentId = x.Student.Id,
                                               StudentName = x.Student.Name
                                           }).ToList();





                return Task.FromResult(response);
            }
            catch (Exception ex)
            {
                throw new SystemException(ex.Message);
            }
        }

        public async Task<StudentToLate> AddStudentToLate(StudentToLateAndEarlyRequest late)
        {
            var userClaim = await http.IsTeacherPicket(userManager, dbContext);
            if (!userClaim.Item1)
                throw new UnauthorizedAccessException("Maaf, Anda tidak sedang piket/anda tidak memiliki akses !");

            DateOnly date = DateOnly.FromDateTime(DateTime.Now);
            var picketToday = dbContext.Picket
                    .Include(x => x.StudentsToLate).ThenInclude(x => x.Student)
                    .Include(x => x.StudentsToLate).ThenInclude(x => x.CreatedBy)
                    .SingleOrDefault(x => x.Date == date);
            if (picketToday == null || DateOnly.FromDateTime(DateTime.Now) != picketToday.Date)
            {
                throw new SystemException("Piket Belum Di buka");
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
        public async Task<StudentComeHomeEarly> AddStudentComeHomeSoEarly(StudentToLateAndEarlyRequest late)
        {
            var userClaim = await http.IsTeacherPicket(userManager, dbContext);
            if (!userClaim.Item1)
                throw new UnauthorizedAccessException("Maaf, Anda tidak sedang piket/anda tidak memiliki akses !");


            DateOnly date = DateOnly.FromDateTime(DateTime.Now);
            var picketToday = dbContext.Picket
                    .Include(x => x.StudentsComeHomeEarly).ThenInclude(x => x.Student)
                    .Include(x => x.StudentsComeHomeEarly).ThenInclude(x => x.CreatedBy)
                    .SingleOrDefault(x => x.Date == date);
            if (picketToday == null || DateOnly.FromDateTime(DateTime.Now) != picketToday.Date)
            {
                throw new SystemException("Piket Belum Di buka");
            }
          
            var soEarly = new StudentComeHomeEarly
            {
                Student = new Student { Id = late.StudentId },
                CreatedBy = userClaim.Item2,
                AttendanceStatus = SharedModel.StudentAttendanceStatus.Present,
                CreateAt = DateTime.Now.ToUniversalTime(),
                Description = late.Description,
                Time = late.AtTime
            };

            dbContext.Entry(picketToday);
            picketToday.StudentsComeHomeEarly.Add(soEarly);
            dbContext.SaveChanges();
            return soEarly;
        }
    }
}
