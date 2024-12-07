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
    public interface IStudentService
    {
        Task<IEnumerable<StudentClassRoom>> GetAlStudentWithClass();
        Task<IEnumerable<StudentClassRoom>> SearchStudent(string searchtext);
        Task<bool> DeleteStudent(int id);
        Task<bool> PutStudent(int id, Student model);
        Task<Student> PostStudent(Student model);
        Task<IEnumerable<Student>> GetAllStudent();
        Task<Student> GetStudentById(int id);

    }

    public class StudentService : IStudentService
    {
        private readonly IHttpContextAccessor http;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly ApplicationDbContext dbContext;
        private readonly ISchoolYearService schoolYearService;
        private static Picket picketToday;

        public StudentService(IHttpContextAccessor _http, UserManager<ApplicationUser> _userManager,
            ApplicationDbContext _dbContext, ISchoolYearService _schoolYearService)
        {
            http = _http;
            userManager = _userManager;
            dbContext = _dbContext;
            schoolYearService = _schoolYearService;
        }

        public async Task<IEnumerable<StudentClassRoom>> GetAlStudentWithClass()
        {
            try
            {

                var schoolYearActive = await schoolYearService.GetActiveSchoolYear();
                if (schoolYearService == null)
                    throw new SystemException("Belum Ada Tahun Ajaran Aktif !");

                List<StudentClassRoom> list = new List<StudentClassRoom>();

                foreach (var item in dbContext.ClassRooms
                    .Include(x => x.SchoolYear)
                    .Include(x => x.Department).Include(x => x.Students)
                    .ThenInclude(x => x.Student)
                    .Where(x => x.SchoolYear.Id == schoolYearActive.Id))
                {
                    var data = item.Students.Select(x => new StudentClassRoom
                    {
                        Gender = x.Student.Gender,
                        Id = x.Student.Id,
                        Name = x.Student.Name,
                        Number = x.Student.Number,
                        Photo = x.Student.Photo,
                        ClassRoomId = item.Id,
                        ClassRoomName = item.Name,
                        DepartmenName = item.Department.Name,
                    });
                    list = data.ToList();
                }
                return list.AsEnumerable();
            }
            catch (Exception)
            {
                throw;
            }
        }


        public async Task<IEnumerable<StudentClassRoom>> SearchStudent(string searchtext)
        {
            try
            {
                var txtSearch = searchtext.ToLower();
                var result = dbContext.Students.Where(x => x.Name.ToLower().Contains(txtSearch)
                || x.Email.ToLower().Contains(txtSearch)
                || x.Number.ToLower().Contains(txtSearch)).ToList();

                var xx = await GetAlStudentWithClass();
                return from r in result
                       join x in xx on r.Id equals x.Id into xGroup
                       from xc in xGroup.DefaultIfEmpty()
                       select new StudentClassRoom
                       {
                           Gender = r.Gender,
                           Id = r.Id,
                           Name = r.Name,
                           Number = r.Number,
                           Photo = r.Photo,
                           ClassRoomId = xc == null ? null : xc.ClassRoomId,
                           ClassRoomName = xc == null ? null : xc.ClassRoomName,
                           DepartmenName = xc == null ? null : xc.DepartmenName,
                       };
            }
            catch (Exception)
            {
                throw;
            }
        }

        public Task<bool> DeleteStudent(int id)
        {
            try
            {
                var result = dbContext.Students.SingleOrDefault(x => x.Id == id);
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

        public Task<bool> PutStudent(int id, Student model)
        {
            try
            {
                var result = dbContext.Students.SingleOrDefault(x => x.Id == id);
                if (result != null)
                {
                    dbContext.Entry(result).CurrentValues.SetValues(model);
                    dbContext.SaveChanges();
                }
                return Task.FromResult(true);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<Student> PostStudent(Student model)
        {
            var trans = dbContext.Database.BeginTransaction();
            try
            {
                if (!string.IsNullOrEmpty(model.Email))
                {
                    var user = new ApplicationUser { Email = model.Email, EmailConfirmed = true, Name = model.Name, UserName = model.Email };
                    var createResult = await userManager.CreateAsync(user, "Password@123");
                    if (createResult.Succeeded)
                    {
                        await userManager.AddToRoleAsync(user, "Student");
                        model.UserId = user.Id;
                    }
                    else
                    {
                        throw new SystemException("User Gagal Dibuat !");
                    }
                }

                var result = dbContext.Students.Add(model);
                dbContext.SaveChanges();
                trans.Commit();
                return model;
            }
            catch (Exception ex)
            {
                trans.Rollback();
                throw;
            }
        }

        public Task<IEnumerable<Student>> GetAllStudent()
        {
            try
            {
                return Task.FromResult(dbContext.Students.AsEnumerable());
            }
            catch (Exception)
            {
                throw;
            }
        }

        public Task<Student> GetStudentById(int id)
        {
            try
            {
                var result = dbContext.Students.SingleOrDefault(x => x.Id == id);
#pragma warning disable CS8619 // Nullability of reference types in value doesn't match target type.
                return Task.FromResult(result);
#pragma warning restore CS8619 // Nullability of reference types in value doesn't match target type.
            }
            catch (Exception)
            {
                throw;
            }
        }


    }
}
