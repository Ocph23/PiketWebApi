
using ErrorOr;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PiketWebApi.Data;
using PiketWebApi.Validators;
using SharedModel.Requests;
using SharedModel.Responses;

namespace PiketWebApi.Services
{
    public interface IScheduleService
    {
        Task<ErrorOr<IEnumerable<ScheduleResponse>>> GetBySchoolYearAsync(int id);
        Task<ErrorOr<IEnumerable<ScheduleResponse>>> GetAsync();
        Task<ErrorOr<ScheduleResponse>> GetByIdAsnyc(int id);
        Task<ErrorOr<ScheduleResponse>> PostAsync(ScheduleRequest req);
        Task<ErrorOr<bool>> PutAsync(int id, ScheduleRequest req);
        Task<ErrorOr<bool>> DeleteAsync(int id);
    }

    public class ScheduleService : IScheduleService
    {
        private IHttpContextAccessor http;
        private UserManager<ApplicationUser> userManager;
        private ApplicationDbContext dbContext;
        private readonly ISchoolYearService schoolYearService;

        public ScheduleService(IHttpContextAccessor _http, UserManager<ApplicationUser> _userManager, ApplicationDbContext _dbContext, ISchoolYearService _schoolYearService)
        {
            http = _http;
            userManager = _userManager;
            dbContext = _dbContext;
            schoolYearService = _schoolYearService;
        }


        public async Task<ErrorOr<IEnumerable<ScheduleResponse>>> GetBySchoolYearAsync(int id)
        {
            try
            {
                var result = from a in dbContext.Schedules
                             .Where(x => x.SchoolYear.Id == id)
                             .Include(x => x.SchoolYear).Include(x => x.Teacher)
                             select new ScheduleResponse(a.Id, a.SchoolYear.Id, a.SchoolYear.Year,
                             a.SchoolYear.Semester,
                             a.DayOfWeek.ToString(), a.Teacher.Id,    
                             a.Teacher.RegisterNumber, a.Teacher.Name,a.Teacher.Photo);

                return await Task.FromResult(result.ToList());
            }
            catch (Exception)
            {
                return Error.Conflict();
            }
        }

        public async Task<ErrorOr<IEnumerable<ScheduleResponse>>> GetAsync()
        {
            try
            {
                var schoolYearActive = await schoolYearService.GetActiveSchoolYear();
                if (schoolYearActive.IsError)
                    return schoolYearActive.Errors;

                return await GetBySchoolYearAsync(schoolYearActive.Value.Id);
            }
            catch (Exception)
            {
                return Error.Conflict();
            }
        }

        public async Task<ErrorOr<ScheduleResponse>> GetByIdAsnyc(int id)
        {
            try
            {
                var result = from a in dbContext.Schedules
                             .Where(x => x.SchoolYear.Id == id)
                             .Include(x => x.SchoolYear).Include(x => x.Teacher)
                             select new ScheduleResponse(a.Id, a.SchoolYear.Id, a.SchoolYear.Year,
                             a.SchoolYear.Semester,
                             a.DayOfWeek.ToString(), a.Teacher.Id,
                             a.Teacher.RegisterNumber, a.Teacher.Name, a.Teacher.Photo);

                if (!result.Any())
                    return Error.Failure("Schedule", "Data jadwal tidak ditemukan");

                return await Task.FromResult(result.FirstOrDefault()!);
            }
            catch (Exception)
            {
                return Error.Conflict();
            }
        }

        public async Task<ErrorOr<ScheduleResponse>> PostAsync(ScheduleRequest req)
        {
            try
            {
                var schoolYearActive = await schoolYearService.GetActiveSchoolYear();
                if (schoolYearActive.IsError)
                    return schoolYearActive.Errors;

                var existsSchedule = dbContext.Schedules
                    .Include(x => x.Teacher)
                    .Include(x => x.SchoolYear)
                    .FirstOrDefault(x => x.SchoolYear.Actived && x.Teacher.Id == req.TeacherId);


                if (existsSchedule != null)
                    return Error.Failure("schedule", $"{existsSchedule.Teacher.Name} sudah terdaftar");
                var model = new Schedule
                {
                    DayOfWeek = (DayOfWeek)req.DayOfWeek,
                    SchoolYear = schoolYearActive.Value,
                    Teacher = new Teacher { Id = req.TeacherId }
                };
                dbContext.Entry(model.Teacher).State = EntityState.Unchanged;
                var result = dbContext.Schedules.Add(model);
                dbContext.SaveChanges();

                var resultResponse = new ScheduleResponse(model.Id, model.SchoolYear.Id,
                    model.SchoolYear.Year, model.SchoolYear.Semester, model.DayOfWeek.ToString(), model.Teacher.Id,
                    model.Teacher.RegisterNumber, model.Teacher.Name, model.Teacher.Photo);
                return await Task.FromResult(resultResponse);
            }
            catch (Exception ex)
            {
                return Error.Conflict();
            }
        }

        public async Task<ErrorOr<bool>> PutAsync(int id, ScheduleRequest req)
        {
            try
            {
                var validator = new Validators.ScheduleValidator();
                var validatorResult = validator.Validate(req);
                if (!validatorResult.IsValid)
                    return validatorResult.GetErrors();

                Schedule? result = dbContext.Schedules.SingleOrDefault(x => x.Id == id);
                if (result == null)
                  return  Error.Failure("Schedule", "Data jadal piket tidak ditemukan.");

                result.DayOfWeek = (DayOfWeek)req.DayOfWeek;
                result.Teacher = new Teacher { Id = req.TeacherId };
                dbContext.Entry(result.Teacher).State = EntityState.Unchanged; ;
                dbContext.SaveChanges();
                return await Task.FromResult(true);
            }
            catch (Exception ex)
            {
                return Error.Conflict();
            }
        }

        public async Task<ErrorOr<bool>> DeleteAsync(int id)
        {

            try
            {
                Schedule? result = dbContext.Schedules.SingleOrDefault(x => x.Id == id);
                if (result == null)
                    Error.Failure("Schedule", "Data jadal piket tidak ditemukan.");

                dbContext.Schedules.Remove(result!);
                dbContext.SaveChanges();
                return await Task.FromResult(true);
            }
            catch (Exception)
            {
                return Error.Conflict();
            }
        }
    }
}
