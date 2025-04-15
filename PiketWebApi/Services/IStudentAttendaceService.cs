using ErrorOr;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PiketWebApi.Data;
using PiketWebApi.Validators;
using SharedModel;
using SharedModel.Requests;
using SharedModel.Responses;

namespace PiketWebApi.Services
{
    public interface IStudentAttendaceService
    {
        Task<ErrorOr<IEnumerable<StudentAttendanceResponse>>> GetAsync();
        Task<ErrorOr<StudentAttendanceResponse>> GetByIdAsync(Guid id);
        Task<ErrorOr<bool>> DeleteAsync(Guid id);
        Task<ErrorOr<bool>> PutAsync(Guid id, StudentAttendenceRequest model);
        Task<ErrorOr<StudentAttendanceResponse>> PostAsync(StudentAttendenceRequest model);
        Task<ErrorOr<IEnumerable<StudentAttendanceSyncRequest>>> SyncData(IEnumerable<StudentAttendanceSyncRequest> data);
        Task<ErrorOr<IEnumerable<StudentAttendanceReportResponse>>> GetAbsenByClassRoomMonthYear(int classroom, int month, int year);
    }

    public class StudentAttendaceService : IStudentAttendaceService
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly ApplicationDbContext dbContext;
        private readonly IStudentService studentService;
        private readonly IPicketService picketService;
        private readonly IHttpClientFactory factory;
        private readonly HttpClient waAppClient;
        private readonly ISchoolYearService schoolYearService;

        public DateTime CreatedAt { get; private set; }

        public StudentAttendaceService(
            UserManager<ApplicationUser> _userManager,
            ApplicationDbContext _dbContext,
              IStudentService _studentService, IPicketService _picketService, IHttpClientFactory _factory
            )
        {
            userManager = _userManager;
            dbContext = _dbContext;
            studentService = _studentService;
            picketService = _picketService;
            factory = _factory;
            this.waAppClient= factory.CreateClient("waapp");
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
                             select new StudentAttendanceResponse(a.Id, a.PicketId,
                             b.Id, b.Name, b.ClassRoomName, b.DepartmenName,
                             a.AttendanceStatus, a.TimeIn, a.TimeOut, a.Description, a.CreateAt);
                return result.ToList();
            }
            catch (Exception)
            {
                return Error.Conflict();
            }
        }

        public async Task<ErrorOr<StudentAttendanceResponse>> GetByIdAsync(Guid id)
        {
            try
            {
                var students = await studentService.GetAlStudentWithClass();
                if (students.IsError)
                    return students.Errors;

                var result = from a in dbContext.StudentAttendaces.Include(x => x.Student).Where(x => x.Id == id).ToList()
                             join b in students.Value on a.Student.Id equals b.Id
                             select new StudentAttendanceResponse(a.Id, a.PicketId,
                             b.Id, b.Name, b.ClassRoomName, b.DepartmenName,
                             a.AttendanceStatus, a.TimeIn, a.TimeOut, a.Description, a.CreateAt);
                return result.FirstOrDefault();
            }
            catch (Exception)
            {
                return Error.Conflict();
            }
        }

        public async Task<ErrorOr<bool>> DeleteAsync(Guid id)
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

        public async Task<ErrorOr<bool>> PutAsync(Guid id, StudentAttendenceRequest model)
        {
            try
            {
                var result = dbContext.StudentAttendaces.SingleOrDefault(x => x.Id == id);
                if (result == null)
                {
                    return Error.NotFound("NotFound", "Data Absen Siswa tidak ditemukan.");
                }
                result.TimeIn = model.TimeIn.Value;
                result.TimeOut = model.TimeOut == null ? null : model.TimeOut.Value;
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

                var model = new StudentAttendance
                {
                    Id = req.Id == Guid.Empty ? Guid.NewGuid() : req.Id,
                    Student = student,
                    PicketId = req.PicketId,
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

                var picket = dbContext.Picket.SingleOrDefault(x => x.Id == req.PicketId);
                //picket.StudentAttendances.Add(model);
                dbContext.StudentAttendaces.Add(model);
                dbContext.SaveChanges();
                return await GetByIdAsync(model.Id);
            }
            catch (Exception)
            {
                return Error.Conflict();
            }
        }

        public async Task<ErrorOr<IEnumerable<StudentAttendanceSyncRequest>>> SyncData(IEnumerable<StudentAttendanceSyncRequest> req)
        {
            try
            {
                var dataToInsert = new List<StudentAttendance>();
                var dataToUpdate = new List<StudentAttendance>();
                foreach (var item in req)
                {
                    var model = new StudentAttendance
                    {
                        Id = item.Id,
                        Student = dbContext.Students.FirstOrDefault(x => x.Id == item.StudentId)!,
                        PicketId = item.PicketId,
                        AttendanceStatus = item.Status,
                        TimeIn = item.TimeIn.Value,
                        TimeOut = item.TimeOut,
                        Description = item.Description,
                        CreateAt = DateTime.Now.ToUniversalTime()
                    };

                    var data = dbContext.StudentAttendaces.FirstOrDefault(x => x.Id == model.Id);
                    if (data == null)
                    {
                        dataToInsert.Add(model);
                        dbContext.Entry(model.Student).State = EntityState.Unchanged;

                       _= SendMessageToStudentparent(model);

                    }
                    else
                    {
                        dbContext.Entry(data).CurrentValues.SetValues(model);
                    }

                }

                if (dataToInsert.Count > 0)
                {
                    dbContext.AddRange(dataToInsert);
                }

                dbContext.SaveChanges();
                foreach (var item in req)
                {
                    item.IsSynced = true;
                }
                return req.ToList();
            }
            catch (Exception ex)
            {
                return Error.Conflict();
            }
        }

        private async Task SendMessageToStudentparent(StudentAttendance model)
        {
            try
            {
                var result = await waAppClient
                    .PostAsJsonAsync("absen", new { to = model.Student.ParentPhoneNumber, message=$"{model.Student.Name} Masuk Jam: {model.TimeIn}" });
            }
            catch (Exception)
            {
                return;
            }
        }

        async Task<ErrorOr<IEnumerable<StudentAttendanceReportResponse>>> IStudentAttendaceService.GetAbsenByClassRoomMonthYear(int classroom, int month, int year)
        {
            try
            {
                var startDate = DateOnly.FromDateTime(new DateTime(year, month, 1));
                var endDate = startDate.AddMonths(1).AddDays(-1);
                var students = await studentService.GetStudentsWithClassRoom(classroom);
                var pickets = dbContext.Picket.Where(x => x.Date >= startDate && x.Date <= endDate)
                .Include(x => x.StudentAttendances).ThenInclude(x => x.Student).AsEnumerable();


                List<StudentAttendanceReportResponse> result = new List<StudentAttendanceReportResponse>();
                foreach (var item in pickets)
                {
                    var att = from a in students.Value
                              join b in item.StudentAttendances on a.Id equals b.Student.Id into bGroup
                              from b in bGroup.DefaultIfEmpty()
                              where a.ClassRoomId == classroom
                              select new StudentAttendanceReportResponse
                              {
                                  StudentId = a.Id,
                                  StudentName = a.Name,
                                  ClassRoomName = a.ClassRoomName,
                                  DepartmentName = a.DepartmenName,
                                  PicketId = item.Id,
                                  PicketDate = item.Date,
                                  Status = b == null ? SharedModel.AttendanceStatus.Alpa : b.AttendanceStatus,
                                  TimeIn = b?.TimeIn,
                                  TimeOut = b == null ? null : b.TimeOut,
                                  Description = b == null ? null : b.Description
                              };

                    result.AddRange(att);
                }

                return result.ToList();

            }
            catch (Exception ex)
            {
                return Error.Conflict();
            }
        }
    }
}
