using SharedModel;
using SharedModel.Models;

namespace SharedModel.Models
{
    public class StudentToLate
    {
        public int Id { get; set; }
        public Student? Student { get; set; }
        public StudentAttendanceStatus AttendanceStatus { get; set; }
         public string? Description { get; set; }
        public Teacher? CreatedBy { get; set; }
        public DateTime CreateAt { get; set; }=DateTime.Now;
        public TimeSpan? AtTime { get; set; } = DateTime.Now.TimeOfDay;
    }
}
