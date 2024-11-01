using SharedModel;
using SharedModel.Models;

namespace SharedModel.Models
{
    public class StudentPicketItem
    {

        public int Id { get; set; }

        public Student Student { get; set; }

        public StudentAttendanceStatus StudentAttendanceStatus { get; set; }
        public List<StudentComeHomeEarly> StudentsComeHomeEarly { get; set; }
    }
}
