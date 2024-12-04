using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;

namespace SharedModel.Models
{

    [Index(nameof(Year),nameof(Semester), IsUnique = true)]
    public class SchoolYear
    {
        public int Id { get; set; }

        public int Year { get; set; }

        [Range(1, 2)]
        public int Semester { get; set; } = 1;
        public string SemesterName => Semester==1 ? "Ganjil" : "Genap";
        public string Name => $"{Year}/{Year + 1}";
        public bool Actived { get; set; }
    }
}
