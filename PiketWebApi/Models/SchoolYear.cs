namespace PiketWebApi.Models
{
    public class SchoolYear
    {
        public int Id { get; set; }
        public int Year { get; set; }
        public string Name => $"{Year}/{Year++}";
        public bool Actived { get; set; }
    }
}
