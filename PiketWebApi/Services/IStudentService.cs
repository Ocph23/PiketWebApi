using ErrorOr;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using PiketWebApi.Data;
using SharedModel.Models;
using SharedModel.Responses;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace PiketWebApi.Services;

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
    Task<ErrorOr<string>> UploadPhoto(int teacherId, byte[] image);
    Task<ErrorOr<PeginationResponse<Student>>> GetAllStudentWithPanitate(string searchTerm, string columnName, string sortOrder, int page, int pagesize);
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
    public async Task<ErrorOr<string>> UploadPhoto(int sturentId, byte[] image)
    {
        try
        {
            if (image.Length <= 0)
                return Error.Validation("Student", "Data file yg anda kirim kosong, periksa kembali file yang anda kirim.");

            var student = dbContext.Students.FirstOrDefault(t => t.Id == sturentId);
            if (student == null)
                return Error.NotFound("Student", "Data siswa tidak ditemukan.");

            var fileName = Path.GetRandomFileName() + ".png";
            if (!Directory.Exists(Helper.StudentPhotoPath))
            {
                Directory.CreateDirectory(Helper.StudentPhotoPath);
            }
            System.IO.File.WriteAllBytes(Helper.StudentPhotoPath + fileName, image);
            if (!string.IsNullOrEmpty(student.Photo))
                Helper.DeleteFile(Helper.StudentPhotoPath + student.Photo);
            student.Photo = fileName;
            dbContext.SaveChanges();
            return fileName;
        }
        catch (Exception ex)
        {
            return Error.Conflict();
        }
    }
    public async Task<ErrorOr<IEnumerable<StudentClassRoom>>> GetAlStudentWithClass()
    {
        try
        {

            var schoolYearActive = await schoolYearService.GetActiveSchoolYear();
            if (schoolYearActive.IsError)
                return schoolYearActive.Errors;

            List<StudentClassRoom> list = new List<StudentClassRoom>();

            foreach (var item in dbContext.ClassRooms
                .Include(x => x.SchoolYear)
                .Include(x => x.Department).Include(x => x.Students)
                .ThenInclude(x => x.Student)
                .Where(x => x.SchoolYear.Id == schoolYearActive.Value.Id))
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
                list.AddRange(data.AsEnumerable());
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
            IEnumerable<Student> result = dbContext.Students.Where(x => x.Name.ToLower().Contains(txtSearch)
            || x.Email!.ToLower().Contains(txtSearch)
            || x.NISN!.ToLower().Contains(txtSearch)
            || x.NIS!.ToLower().Contains(txtSearch)).ToList();

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
                           NISN = r.NISN,
                           Photo = r.Photo,
                           ClassRoomId = xc == null ? null : xc.ClassRoomId,
                           ClassRoomName = xc == null ? null : xc.ClassRoomName,
                           DepartmenName = xc == null ? null : xc.DepartmenName,
                           DepartmenId = xc == null ? null : xc.DepartmenId,
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
        var trans = dbContext.Database.BeginTransaction();
        try
        {
            var validator = new Validators.StudentValidator();
            var validatorResult = validator.Validate(model);
            if (!validatorResult.IsValid)
                return validatorResult.GetErrors();
            var result = dbContext.Students.SingleOrDefault(x => x.Id == id);
            if (result == null)
            {
                return Error.NotFound("Data siswa tidak ditemukan.");
            }

            if (!string.IsNullOrEmpty(model.Email) && string.IsNullOrEmpty(result.Email))
            {

                var userResult = await Helper.CreateUser(userManager,
                    new ApplicationUser { Email = model.Email, EmailConfirmed = true, Name = model.Name, UserName = model.Email }, "Student");
                if (userResult.IsError)
                {
                    trans.Rollback();
                    return userResult.Errors;
                }
                model.UserId = userResult.Value.Id;
            }

            dbContext.Entry(result).CurrentValues.SetValues(model);
            dbContext.SaveChanges();
            trans.Commit();
            return await Task.FromResult(true);
        }
        catch (Exception ex)
        {
            trans.Rollback();
            return Error.Conflict();
        }
    }

    public async Task<ErrorOr<Student>> PostStudent(Student model)
    {
        var validator = new Validators.StudentValidator();
        var validatorResult = validator.Validate(model);
        if (!validatorResult.IsValid)
            return validatorResult.GetErrors();


        var trans = dbContext.Database.BeginTransaction();
        try
        {
            if (!string.IsNullOrEmpty(model.Email))
            {
                var userResult = await Helper.CreateUser(userManager,
                    new ApplicationUser { Email = model.Email, EmailConfirmed = true, Name = model.Name, UserName = model.Email },
                    "Student");
                if (userResult.IsError)
                    return userResult.Errors;
                model.UserId = userResult.Value.Id;
            }
            var result = dbContext.Students.Add(model);
            dbContext.SaveChanges();
            trans.Commit();
            return model;
        }
        catch (Exception ex)
        {
            trans.Rollback();
            return Error.Conflict();
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
            if (result == null)
                return Error.NotFound("NotFound", "Data siswa tidak ditemukan");
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
                 .FirstOrDefault(x => x.SchoolYear.Id == schoolYearActive.Value.Id && x.Students.Any(x => x.Student.Id == studentId));

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
            return Error.Conflict();
        }
    }

    public async Task<ErrorOr<PeginationResponse<Student>>> GetAllStudentWithPanitate(string searchTerm, string columnName, string sortOrder, int page, int pagesize)
    {
        try
        {
            IQueryable<Student> iq = dbContext.Students;
            if (!string.IsNullOrEmpty(searchTerm))
            {
                iq = iq.Where(x => x.Name.ToLower().Contains(searchTerm.ToLower()) ||
                x.PlaceOfBorn.ToLower().Contains(searchTerm.ToLower()) ||
                x.NIS.ToLower().Contains(searchTerm.ToLower()) ||
                x.NISN.ToLower().Contains(searchTerm.ToLower()));
            }

            iq = iq.GetStudentOrder(columnName, sortOrder);

            var totalSize = await iq.CountAsync();

            var data = await iq.Skip((page - 1) * pagesize)
                .Take(pagesize)
                .ToListAsync();

            return new PeginationResponse<Student>(data, page, pagesize, totalSize);

        }
        catch (Exception ex)
        {
            return Error.Conflict();
        }
    }

   

  
}


public static partial class Appextention
{
    public static IQueryable<Student> GetStudentOrder(this IQueryable<Student> query, string columnName, string sortOrder)
    {
        if (sortOrder?.ToLower() == "desc")
        {
            return columnName?.ToLower() switch
            {
                "name" => query.OrderByDescending(x => x.Name),
                "nis" => query.OrderByDescending(x => x.NIS),
                "nisn" => query.OrderByDescending(x => x.NISN),
                _ => query.OrderByDescending(x => x.Id)
            };
        }
        else
        {
            return columnName?.ToLower() switch
            {
                "name" => query.OrderBy(x => x.Name),
                "nis" => query.OrderBy(x => x.NIS),
                "nisn" => query.OrderBy(x => x.NISN),
                _ => query.OrderBy(x => x.Id)
            };
        }
    }
}
