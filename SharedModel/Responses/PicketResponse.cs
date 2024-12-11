using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedModel.Responses
{

    public class PicketResponse
    {
        public int Id { get; set; }
        public DateOnly Date { get; set; }
        public Weather Weather { get; set; }
        public TimeSpan? StartAt { get; set; }
        public TimeSpan? EndAt { get; set; }
        public int CreatedId { get; set; }
        public string? CreatedName { get; set; }
        public string? CreatedNumber { get; set; }
        public DateTime CreateAt { get; set; } = DateTime.Now.ToUniversalTime();
        public ICollection<TeacherAttendanceResponse> TeacherAttendance { get; set; } = default;
        public ICollection<StudentToLateAndComeHomeSoEarlyResponse> StudentsComeHomeEarly { get; set; } = default;
        public ICollection<StudentToLateAndComeHomeSoEarlyResponse> StudentsToLate { get; set; } = default;
    }

    public class StudentToLateAndComeHomeSoEarlyResponse
    {
        public int Id { get; set; }
        public int StudentId { get; set; }
        public string? StudentName { get; set; }
        
        public string? StudentPhoto { get; set; }
        public AttendanceStatus AttendanceStatus { get; set; }
        public string? Description { get; set; }
        public int TeacherId { get; set; }
        public string? TeacherName { get; set; }
        public string? TeacherPhoto { get; set; }
        public DateTime CreateAt { get; set; } = DateTime.Now;
        public TimeSpan? AtTime { get; set; } = DateTime.Now.TimeOfDay;
        public int? ClassRoomId { get; set; }
        public string? ClassRoomName { get; set; }
        public int? DepartmentId { get; set; }
        public string? DepartmentName { get; set; }
        public string? StudentInitial => GetInitial(StudentName);
        public string? TeacherInitial => GetInitial(TeacherName);

        private string? GetInitial(string? name)
        {
            if (string.IsNullOrEmpty(name))
                return string.Empty;

            StringBuilder sb = new StringBuilder();
            foreach (var item in name.Split(" "))
            {
                sb.Append(item.Substring(0, 1).ToUpper());
            }
            return sb.ToString();
        }


    }
    public class TeacherAttendanceResponse
    {
        public int Id { get; set; }
        public AttendanceStatus AttendanceStatus { get; set; }
        public string? Description { get; set; }
        public int TeacherId { get; set; }
        public string? TeacherName { get; set; }
        public string? TeacherPhoto { get; set; }
        public DateTime CreateAt { get; set; } = DateTime.Now;
        public TimeSpan? AtTime { get; set; } = DateTime.Now.TimeOfDay;
        public string? TeacherInitial => GetInitial(TeacherName);

        private string? GetInitial(string? name)
        {
            if (string.IsNullOrEmpty(name))
                return string.Empty;

            StringBuilder sb = new StringBuilder();
            foreach (var item in name.Split(" "))
            {
                sb.Append(item.Substring(0, 1).ToUpper());
            }
            return sb.ToString();
        }


    }






}
