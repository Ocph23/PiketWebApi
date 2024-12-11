
using SharedModel;

namespace PiketWebApi.Data
{
   
    public class LateAndGoHomeEarly
    {
        public int Id { get; set; }
        public Student? Student { get; set; }
        public DateTime CreateAt { get; set; }=DateTime.Now;
        public TimeSpan? Time { get; set; } = DateTime.Now.TimeOfDay;
        public LateAndGoHomeEarlyAttendanceStatus AttendanceStatus { get; set; }
         public string? Description { get; set; }
        public Teacher? CreatedBy { get; set; }
    }
}
