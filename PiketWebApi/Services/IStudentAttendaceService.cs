using ErrorOr;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PiketWebApi.Data;
using PiketWebApi.Validators;
using SharedModel.Requests;
using SharedModel.Responses;

namespace PiketWebApi.Services
{
    public interface IStudentAttendaceService
    {
        Task<ErrorOr<IEnumerable<StudentAttendanceResponse>>> GetAsync();
        Task<ErrorOr<StudentAttendanceResponse>> GetByIdAsync(int id);
        Task<ErrorOr<bool>> DeleteAsync(int id);
        Task<ErrorOr<bool>> PutAsync(int id, StudentAttendenceRequest model);
        Task<ErrorOr<StudentAttendanceResponse>> PostAsync(StudentAttendenceRequest model);
    }

    public class StudentAttendaceService : IStudentAttendaceService
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly ApplicationDbContext dbContext;
        private readonly IStudentService studentService;
        private readonly ISchoolYearService schoolYearService;

        public DateTime CreatedAt { get; private set; }

        public StudentAttendaceService(
            UserManager<ApplicationUser> _userManager,
            ApplicationDbContext _dbContext,
              IStudentService _studentService
            )
        {
            userManager = _userManager;
            dbContext = _dbContext;
            studentService = _studentService;
        }

        public async Task<ErrorOr<IEnumerable<StudentAttendanceResponse>>> GetAsync()
        {
            try
            {
                var students = await studentService.GetAlStudentWithClass();
                if (students.IsError)
                    return students.Errors;

                var result = from a in dbContext.StudentAttendaces.Include(x => x.Student).AsEnumerable()
                             join b in students.Value on a.Student.Id equals b.Id
                             select new StudentAttendanceResponse(a.Id,
                             b.Id, b.Name, b.ClassRoomName, b.DepartmenName,
                             a.AttendanceStatus, a.TimeIn, a.TimeOut, a.Description, a.CreateAt);
                return result.ToList();
            }
            catch (Exception)
            {
                return Error.Conflict();
            }
        }

        public async Task<ErrorOr<StudentAttendanceResponse>> GetByIdAsync(int id)
        {
            try
            {
                var students = await studentService.GetAlStudentWithClass();
                if (students.IsError)
                    return students.Errors;

                var result = from a in dbContext.StudentAttendaces.Include(x => x.Student).Where(x => x.Id == id).ToList()
                             join b in students.Value on a.Student.Id equals b.Id
                             select new StudentAttendanceResponse(a.Id,
                             b.Id, b.Name, b.ClassRoomName, b.DepartmenName,
                             a.AttendanceStatus, a.TimeIn, a.TimeOut, a.Description, a.CreateAt);
                return result.FirstOrDefault();
            }
            catch (Exception)
            {
                return Error.Conflict();
            }
        }

        public async Task<ErrorOr<bool>> DeleteAsync(int id)
        {
            try
            {
                var result = dbContext.StudentAttendaces.SingleOrDefault(x => x.Id == id);
                if (result == null)
                    return Error.NotFound("NotFound", "Data absen tidak ditemukan.");
                dbContext.Remove(result);
                dbContext.SaveChanges();
                return await Task.FromResult(true);
            }
            catch (Exception)
            {
                return Error.Conflict();
            }
        }

        public async Task<ErrorOr<bool>> PutAsync(int id, StudentAttendenceRequest model)
        {
            try
            {
                var result = dbContext.StudentAttendaces.SingleOrDefault(x => x.Id == id);
                if (result == null)
                {
                    return Error.NotFound("NotFound", "Data Absen Siswa tidak ditemukan.");
                }
                result.TimeIn = model.TimeIn.Value;
                result.TimeOut = model.TimeOut==null?null:model.TimeOut.Value;
                result.AttendanceStatus = model.Status;
                result.Description = model.Description;
                dbContext.SaveChanges();
                return await Task.FromResult(true);
            }
            catch (Exception)
            {
                return Error.Conflict();
            }
        }

        public async Task<ErrorOr<StudentAttendanceResponse>> PostAsync(StudentAttendenceRequest req)
        {
            try
            {
                var student = dbContext.Students.SingleOrDefault(x => x.Id == req.StudentId);
                if (student == null)
                    return Error.Conflict("StudentAttendance", $"Data siswa tidak ditemukan !");

                var now = DateOnly.FromDateTime(DateTime.Now);
                if (dbContext.StudentAttendaces.Any(x => x.Student.Id == student.Id && DateOnly.FromDateTime(x.TimeIn) == now))
                    return Error.Conflict("Exists StudentAttendance", $"{student.Name} sudah absen !");

                var model = new StudentAttendace
                {
                    Student = student,
                    AttendanceStatus = req.Status,
                    TimeIn = req.TimeIn.Value.ToUniversalTime(),
                    TimeOut = req.TimeOut == null ? null : req.TimeOut.Value.ToUniversalTime(),
                    Description = req.Description,
                    CreateAt = DateTime.Now.ToUniversalTime()
                };

                var validator = new StudentAttendanceValidator();
                ValidationResult validateResult = validator.Validate(model);
                if (!validateResult.IsValid)
                {
                    return validateResult.GetErrors();
                }

                dbContext.StudentAttendaces.Add(model);
                dbContext.SaveChanges();
                return await GetByIdAsync(model.Id);
            }
            catch (Exception)
            {
                return Error.Conflict();
            }
        }
    }
}
