using System.Text.Json.Serialization;

namespace PiketWebApi.Models
{
    public class Student
    {
        public int Id { get; set; }
        public string? Number { get; set; }
        public string? Name { get; set; }
        public Gender Gender { get; set; }
        public string? PlaceOfBorn  { get; set; }
        [JsonConverter(typeof(DateOnlyJsonConverter))]
        public DateOnly? DateOfBorn { get; set; }
        public string? Email { get; set; }
        public string? Description { get; set; }
        public string? UserId { get; set; }

    }
}
