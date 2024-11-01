namespace SharedModel.Models
{
    public class Schedule
    {
        public int Id { get; set; }
        public SchoolYear SchoolYear{ get; set; }
        public DayOfWeek DayOfWeek { get; set; }
        public Teacher Teacher { get; set; }
    }
}
