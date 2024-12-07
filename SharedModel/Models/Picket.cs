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
        public DateTime CreateAt { get; set; } = DateTime.Now.ToUniversalTime();
        public ICollection<Teacher> TeacherAttendance { get; set; } = default;
        public ICollection<StudentComeHomeEarly> StudentsComeHomeEarly { get; set; } = default;
        public ICollection<StudentToLate> StudentsToLate { get; set; } = default;
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
