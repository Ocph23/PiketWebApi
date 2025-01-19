using Microsoft.EntityFrameworkCore;
using SharedModel;

namespace PiketWebApi.Data  
{

    [Index(nameof(Date))]
    public class Picket
    {
        public int Id { get; set; }
        public DateOnly Date { get; set; }
        public Weather Weather { get; set; }
        public TimeSpan? StartAt { get; set; }
        public TimeSpan? EndAt { get; set; }
        public Teacher? CreatedBy { get; set; }
        public DateTime CreateAt { get; set; } = DateTime.Now.ToUniversalTime();
        public ICollection<TeacherAttendance> TeacherAttendance { get; set; } = default;
        public ICollection<LateAndGoHomeEarly> LateAndComeHomeEarly { get; set; } = default;
        public ICollection<DailyJournal> DailyJournals { get; set; } = default;
        public static Picket? Create(Teacher teacher)
        {
            return new Picket()
            {
                CreateAt = DateTime.Now.ToUniversalTime(),
                CreatedBy = teacher,
                Date = DateOnly.FromDateTime(DateTime.Now),                         
                StartAt = new TimeSpan(7, 15, 0),
                EndAt= new TimeSpan(15, 00, 0), Weather= Weather.Cerah
            };
        }
    }
}
