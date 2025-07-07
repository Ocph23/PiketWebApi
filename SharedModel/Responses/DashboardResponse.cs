using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedModel.Responses
{
    public class DashboardResponse
    {
        public int TotalStudents { get; set; }
        public int TotalTeachers { get; set; }
        public int TotalClassrooms { get; set; }
        public int TotalMaleStudents { get; set; }
        public int TotalFemaleStudents { get; set; }
        public int TotalMaleTeachers { get; set; }
        public int TotalFemaleTeachers { get; set; }
        public int TotalDepartments { get; set; }
        public string SchoolYearName { get; set; }

        public ICollection<Kehadiran> Kehadirans { get; set; } = new List<Kehadiran>();
        public int SchoolYear { get; set; }
        public int SchoolYearId { get; set; }
        public int Semester { get; set; }
        public string SemesterName { get; set; }
    }

    public class Kehadiran
    {
        public string GroupName { get; set; }
        public IEnumerable<StudentAttendanceReportResponse> Data { get; set; }
    }
}
