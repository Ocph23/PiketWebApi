using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;

namespace PiketWebApi.Data
{

    [Index(nameof(NIS))]
    public class Student : Profile
    {
        public string NIS { get; set; }
        public string? NISN { get; set; }

    }
}
