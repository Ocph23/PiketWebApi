﻿using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SharedModel.Models;

namespace PiketWebApi.Data
{
    public class ApplicationDbContext: IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)   :base(options)
        {
        }
        public DbSet<ClassRoom> ClassRooms { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<SchoolYear> SchoolYears { get; set; }
        public DbSet<Teacher> Teachers { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<Schedule> Schedules { get; set; }
        public DbSet<Picket> Picket{ get; set; }
        public DbSet<StudentAttendance> StudentAttendaces { get; set; }
        public DbSet<TeacherAttendance> TeacherAttendances{ get; set; }
    }
}
