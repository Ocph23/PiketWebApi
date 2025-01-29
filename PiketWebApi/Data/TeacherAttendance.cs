using SharedModel;
using SharedModel.Models;

namespace PiketWebApi.Data
{
   
    public class TeacherAttendance
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Teacher? Teacher { get; set; }
        public  DateTime TimeIn { get; set; }   
        public  DateTime? TimeOut{ get; set; }
        public AttendanceStatus AttendanceStatus { get; set; } = AttendanceStatus.Hadir;
         public string? Description { get; set; }
        public DateTime CreateAt { get; set; }=DateTime.Now;
    }
}
