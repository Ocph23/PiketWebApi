using SharedModel.Models;
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
        public TimeOnly? StartAt { get; set; }
        public TimeOnly? EndAt { get; set; }
        public int CreatedId { get; set; }
        public string? CreatedName { get; set; }
        public string? CreatedNumber { get; set; }
        public DateTime CreateAt { get; set; } = DateTime.Now.ToUniversalTime();
        public IEnumerable<Teacher> TeacherAttendance { get; set; } = default;
        public IEnumerable<StudentToLateAndComeHomeSoEarlyResponse> StudentsComeHomeEarly { get; set; } = default;
        public IEnumerable<StudentToLateAndComeHomeSoEarlyResponse> StudentsToLate { get; set; } = default;
    }

    public class StudentToLateAndComeHomeSoEarlyResponse
    {
        public int Id { get; set; }
        public int StudentId { get; set; }
        public string? StudentName { get; set; }
        public string? StudentPhoto { get; set; }
        public StudentAttendanceStatus AttendanceStatus { get; set; }
        public string? Description { get; set; }
        public int TeacherId { get; set; }
        public string? TeacherName{ get; set; }
        public string? TeacherPhoto { get; set; }
        public DateTime CreateAt { get; set; } = DateTime.Now;
        public TimeSpan? AtTime { get; set; } = DateTime.Now.TimeOfDay;
        public int? ClassRoomId { get; set; }
        public string? ClassRoomName { get; set; }
        public int? DepartmentId { get; set; }
        public string? DepartmentName { get; set; }
    }
   





}
