using ErrorOr;
using Microsoft.EntityFrameworkCore;
using PiketWebApi.Data;
using SharedModel.Responses;

namespace PiketWebApi.Services
{
    public interface IDashboardService
    {
        Task<ErrorOr<DashboardResponse>> Get();
    }


    public class DashboardService : IDashboardService
    {
        private readonly ApplicationDbContext dbContext;
        public DashboardService(ApplicationDbContext _dbContext)
        {
            dbContext = _dbContext;
        }
        public async Task<ErrorOr<DashboardResponse>> Get()
        {
            // Implement the logic to retrieve dashboard data
            var lastSchoolYear = await dbContext.SchoolYears
                .FirstOrDefaultAsync(x=>x.Actived);


            if (lastSchoolYear == null)
                return Error.NotFound("No active school year found.");

            var dashboard = new DashboardResponse
            {
                SchoolYearId= lastSchoolYear.Id,
                SchoolYear= lastSchoolYear.Year,
                SchoolYearName = lastSchoolYear.Name,
                Semester = lastSchoolYear.Semester,
                SemesterName = lastSchoolYear.SemesterName,
                TotalStudents = await dbContext.Students.CountAsync(x => x.Status == SharedModel.StudentStatus.Aktif),
                TotalMaleStudents = await dbContext.Students.CountAsync(x => x.Status == SharedModel.StudentStatus.Aktif && x.Gender == SharedModel.Gender.Pria),
                TotalFemaleStudents = await dbContext.Students.CountAsync(x => x.Status == SharedModel.StudentStatus.Aktif && x.Gender == SharedModel.Gender.Wanita),
                TotalTeachers = await dbContext.Teachers.CountAsync(),
                TotalMaleTeachers = await dbContext.Teachers.CountAsync(x => x.Gender == SharedModel.Gender.Pria),
                TotalFemaleTeachers = await dbContext.Teachers.CountAsync(x => x.Gender == SharedModel.Gender.Wanita),
                TotalClassrooms = await dbContext.ClassRooms.CountAsync(x => x.SchoolYear.Id == lastSchoolYear.Id),
                TotalDepartments = await dbContext.Departments.CountAsync(),
            };



            var result = from p in dbContext.Picket.Where(x => x.SchoolYearId == lastSchoolYear.Id).Include(x => x.SchoolYear)
                         join a in dbContext.StudentAttendaces on p.Id equals a.PicketId into xattendate
                         from a in xattendate.DefaultIfEmpty()
                         join s in dbContext.Students on a.StudentId equals s.Id into x
                         from s in x.DefaultIfEmpty()
                         select new StudentAttendanceReportResponse
                         {
                             StudentId = s == null ? null : s.Id,
                             StudentName = s == null ? null : s.Name,
                             PicketId = p.Id,
                             PicketDate = p.Date,
                             SchoolYearId = p.SchoolYearId,
                             Status = a == null ? SharedModel.AttendanceStatus.Alpa : a.AttendanceStatus,
                             TimeIn = a == null ? null : a.TimeIn,
                             TimeOut = a == null ? null : a.TimeOut,
                             Description = a == null ? null : a.Description
                         };


            var group = result.GroupBy(x => new {x.PicketDate.Month,x.PicketDate.Year });

            foreach (var item in group)
            {
                dashboard.Kehadirans.Add(new Kehadiran { GroupName = $"{item.Key.Month}-{item.Key.Year}", Data = item });
            }


            return dashboard;
        }
    }
}
