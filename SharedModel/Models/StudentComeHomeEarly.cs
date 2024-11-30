using SharedModel.Models;

namespace SharedModel.Models
{
    public class StudentComeHomeEarly
    {
        public int Id { get; set; }
        public Student Student { get; set; }
        public TimeSpan Time { get; set; }
        public DateTime CreateAt { get; set; }
        public StudentAttendanceStatus AttendanceStatus { get; set; }
        public string? Description { get; set; }
        public Teacher CreatedBy { get; set; }

    }
}
