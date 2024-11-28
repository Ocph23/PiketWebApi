using Microsoft.EntityFrameworkCore;
using System;

namespace SharedModel.Models
{

    [Index(nameof(Year), IsUnique = true)]
    public class SchoolYear
    {
        public int Id { get; set; }

        public int Year { get; set; }
        public int Semester { get; set; } = 1;
        public string Name => $"{Year}/{Year + 1}";
        public bool Actived { get; set; }
    }
}
