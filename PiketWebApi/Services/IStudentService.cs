﻿using ErrorOr;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PiketWebApi.Data;

namespace PiketWebApi.Services
{
    public interface IStudentService
    {
        Task<ErrorOr<IEnumerable<StudentClassRoom>>> GetAlStudentWithClass();
        Task<ErrorOr<IEnumerable<StudentClassRoom>>> SearchStudent(string searchtext);
        Task<ErrorOr<bool>> DeleteStudent(int id);
        Task<ErrorOr<bool>> PutStudent(int id, Student model);
        Task<ErrorOr<Student>> PostStudent(Student model);
        Task<ErrorOr<IEnumerable<Student>>> GetAllStudent();
        Task<ErrorOr<Student>> GetStudentById(int id);
        Task<ErrorOr<StudentClassRoom?>> GetStudentWithClass(int id);
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

        public async Task<ErrorOr<IEnumerable<StudentClassRoom>>> GetAlStudentWithClass()
        {
            try
            {

                var schoolYearActive = await schoolYearService.GetActiveSchoolYear();
                if (schoolYearService == null)
                    return Error.Failure("Belum Ada Tahun Ajaran Aktif !");

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
                        NIS = x.Student.NIS,
                        NISN = x.Student.NISN,
                        Photo = x.Student.Photo,
                        ClassRoomId = item.Id,
                        ClassRoomName = item.Name,
                        DepartmenId = item.Department.Id,
                        DepartmenName = item.Department.Name,
                    });
                    list = data.ToList();
                }
                return await Task.FromResult(list.ToList());
            }
            catch (Exception)
            {
                return Error.Conflict();
            }
        }
        public async Task<ErrorOr<IEnumerable<StudentClassRoom>>> SearchStudent(string searchtext)
        {
            try
            {
                var txtSearch = searchtext.ToLower();
                IEnumerable<Student>? result = dbContext.Students.Where(x => x.Name.ToLower().Contains(txtSearch)
                || x.Email!.ToLower().Contains(txtSearch)
                || x.NISN!.ToLower().Contains(txtSearch)
                || x.NIS!.ToLower().Contains(txtSearch)).AsEnumerable();

                var xx = await GetAlStudentWithClass();
                if (xx.IsError)
                    return xx.Errors;

                var data = from r in result
                           join x in xx.Value on r.Id equals x.Id into xGroup
                           from xc in xGroup.DefaultIfEmpty()
                           select new StudentClassRoom
                           {
                               Gender = r.Gender,
                               Id = r.Id,
                               Name = r.Name,
                               NIS = r.NIS,
                               NISN = xc.NISN,
                               Photo = r.Photo,
                               ClassRoomId = xc == null ? null : xc.ClassRoomId,
                               ClassRoomName = xc == null ? null : xc.ClassRoomName,
                               DepartmenName = xc == null ? null : xc.DepartmenName,
                           };
                return data.ToList();
            }
            catch (Exception)
            {
                return Error.Conflict();
            }
        }

        public async Task<ErrorOr<bool>> DeleteStudent(int id)
        {
            try
            {
                var result = dbContext.Students.SingleOrDefault(x => x.Id == id);
                if (result == null)
                    return Error.NotFound("Data siswa tidak ditemukan.");


                dbContext.Remove(result);
                dbContext.SaveChanges();
                return await Task.FromResult(true);
            }
            catch (Exception)
            {
                return Error.Conflict();
            }
        }

        public async Task<ErrorOr<bool>> PutStudent(int id, Student model)
        {
            try
            {
                var result = dbContext.Students.SingleOrDefault(x => x.Id == id);
                if (result == null)
                {
                    return Error.NotFound("Data siswa tidak ditemukan.");
                }
                dbContext.Entry(result).CurrentValues.SetValues(model);
                dbContext.SaveChanges();
                return await Task.FromResult(true);
            }
            catch (Exception)
            {
                return Error.Conflict();
            }
        }

        public async Task<ErrorOr<Student>> PostStudent(Student model)
        {
            var validator = new Validators.StudentValidator();




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
                        return Error.Failure("User Gagal Dibuat !");
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

        public async Task<ErrorOr<IEnumerable<Student>>> GetAllStudent()
        {
            try
            {
                return await Task.FromResult(dbContext.Students.ToList());
            }
            catch (Exception)
            {
                return Error.Conflict();
            }
        }

        public async Task<ErrorOr<Student>> GetStudentById(int id)
        {
            try
            {
                var result = dbContext.Students.SingleOrDefault(x => x.Id == id);
                return await Task.FromResult(result);
            }
            catch (Exception)
            {
                return Error.Conflict();
            }
        }

        public async Task<ErrorOr<StudentClassRoom?>> GetStudentWithClass(int studentId)
        {
            try
            {
                var schoolYearActive = await schoolYearService.GetActiveSchoolYear();
                if (schoolYearService == null)
                    return Error.Failure("Belum Ada Tahun Ajaran Aktif !");

                var item = dbContext.ClassRooms
                     .Include(x => x.SchoolYear)
                     .Include(x => x.Department)
                     .Include(x => x.Students)
                     .ThenInclude(x => x.Student)
                     .FirstOrDefault(x => x.SchoolYear.Id == schoolYearActive.Id && x.Students.Any(x => x.Student.Id == studentId));

                if (item == null)
                    return Error.NotFound();

                return item.Students.Select(x => new StudentClassRoom
                {
                    Gender = x.Student.Gender,
                    Id = x.Student.Id,
                    Name = x.Student.Name,
                    NIS = x.Student.NIS,
                    NISN = x.Student.NISN,
                    Photo = x.Student.Photo,
                    ClassRoomId = item.Id,
                    ClassRoomName = item.Name,
                    DepartmenId = item.Department.Id,
                    DepartmenName = item.Department.Name,
                }).FirstOrDefault();

            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
