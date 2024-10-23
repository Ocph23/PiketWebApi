namespace PiketWebApi.Models
{
    public class ClassRoom
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public SchoolYear SchoolYear { get; set; }
        public Department Department{ get; set; }
        public Student ClassLeader { get; set; }
        public Teacher HomeroomTeacher { get; set; }
        public ICollection<Student> Students { get; set; } = new List<Student>();
    }
}
