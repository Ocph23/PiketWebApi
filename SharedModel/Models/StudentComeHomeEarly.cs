using SharedModel.Models;

namespace SharedModel.Models
{
    public class StudentComeHomeEarly
    {
        public int Id { get; set; }

        public Student Student { get; set; }

        public TimeOnly Time { get; set; }

        public DateTime CreateAt { get; set; }
        public Teacher CreatedBy { get; set; }

    }
}
