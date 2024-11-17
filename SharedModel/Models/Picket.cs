using SharedModel;

namespace SharedModel.Models
{
    public class Picket
    {
        public int Id { get; set; }
        public DateOnly Date { get; set; }
        public Weather Weather { get; set; }
        public TimeOnly? StartAt { get; set; }
        public TimeOnly? EndAt { get; set; }
        public Teacher? CreatedBy { get; set; }
        public DateTime CreateAt { get; set; }
        public List<Teacher> TeacherAttendance { get; set; } = new();
        public List<StudentComeHomeEarly> StudentsComeHomeEarly { get; set; } = new();
        public List<StudentToLate> StudentsToLate { get; set; } = new();
        public static Picket? Create(Teacher teacher)
        {
            return new Picket()
            {
                CreateAt = DateTime.Now.ToUniversalTime(),
                CreatedBy = teacher,
                Date = DateOnly.FromDateTime(DateTime.Now),
                StartAt = TimeOnly.FromTimeSpan(new TimeSpan(7, 15, 0))
            };
        }
    }
}
