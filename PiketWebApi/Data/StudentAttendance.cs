using SharedModel;

namespace PiketWebApi.Data
{
    public class StudentAttendance
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public int PicketId { get; set; }
        public int StudentId { get; set; }
        public Student Student { get; set; }
        public DateTime TimeIn { get; set; }
        public DateTime? TimeOut { get; set; }
        public AttendanceStatus AttendanceStatus { get; set; } = AttendanceStatus.Hadir;
        public string? Description { get; set; }
        public DateTime CreateAt { get; set; } = DateTime.Now;
    }
}
