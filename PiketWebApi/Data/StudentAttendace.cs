using SharedModel;

namespace PiketWebApi.Data
{
    public class StudentAttendace
    {
        public int Id { get; set; }
        public Student Student { get; set; }
        public DateTime TimeIn { get; set; }
        public DateTime? TimeOut { get; set; }
        public AttendanceStatus AttendanceStatus { get; set; } = AttendanceStatus.Hadir;
        public string? Description { get; set; }
        public DateTime CreateAt { get; set; } = DateTime.Now;
    }
}
