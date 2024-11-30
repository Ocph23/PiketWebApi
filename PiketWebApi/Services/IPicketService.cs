using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PiketWebApi.Data;
using SharedModel.Models;
using SharedModel.Requests;
using System.Net.Sockets;
using System.Security.Claims;

namespace PiketWebApi.Services
{
    public interface IPicketService
    {
        Task<StudentComeHomeEarly> AddStudentComeHomeSoEarly(StudentToLateAndEarlyRequest early);
        Task<StudentToLate> AddStudentToLate(StudentToLateAndEarlyRequest late);
        Task<Picket> CreateNewPicket();
        Task<Picket> GetPicketToday();
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

        public Task<Picket> GetPicketToday()
        {
            try
            {
                if (picketToday == null)
                {
                    DateOnly date = DateOnly.FromDateTime(DateTime.Now);
                    picketToday = dbContext.Picket
                        .Include(x => x.CreatedBy)
                        .Include(x => x.TeacherAttendance)
                        .Include(x => x.StudentsToLate)
                        .Include(x => x.StudentsComeHomeEarly)
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
                return Task.FromResult(picketToday);
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

            var picketToday = await GetPicketToday();
            if (picketToday == null)
                throw new SystemException("Picket Belum Dibuka.");

            var toLate = new StudentToLate
            {
                Student = new Student { Id = late.StudentId },
                CreatedBy = userClaim.Item2,
                AttendanceStatus = SharedModel.StudentAttendanceStatus.Present,
                CreateAt = DateTime.Now.ToUniversalTime(),
                Description = late.Description,
                AtTime = late.AtTime
            };

            dbContext.Entry(picketToday);
            picketToday.StudentsToLate.Add(toLate);
            dbContext.SaveChanges();
            return toLate;
        }
        public async Task<StudentComeHomeEarly> AddStudentComeHomeSoEarly(StudentToLateAndEarlyRequest late)
        {
            var userClaim = await http.IsTeacherPicket(userManager, dbContext);
            if (!userClaim.Item1)
                throw new UnauthorizedAccessException("Maaf, Anda tidak sedang piket/anda tidak memiliki akses !");

            var picketToday = await GetPicketToday();
            if (picketToday == null)
                throw new SystemException("Picket Belum Dibuka.");

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
