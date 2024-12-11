using SharedModel;
using SharedModel.Models;

namespace PiketWebApi.Data
{
   
    public class TeacherAttendance
    {
        public int Id { get; set; }
        public Teacher? Teacher { get; set; }
        public TimeSpan? Time { get; set; } = DateTime.Now.TimeOfDay;
        public AttendanceStatus AttendanceStatus { get; set; }
         public string? Description { get; set; }
        public DateTime CreateAt { get; set; }=DateTime.Now;
    }
}
