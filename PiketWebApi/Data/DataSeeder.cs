using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace PiketWebApi.Data
{
    public static class DataSeeder
    {
        public static async Task SeedData(WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            var dbcontext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            await dbcontext.Database.MigrateAsync(); // Apply pending migrations

            var userManager = scope.ServiceProvider.GetService<UserManager<ApplicationUser>>();
            var roleManager = scope.ServiceProvider.GetService<RoleManager<IdentityRole>>();

            if (!dbcontext.Roles.Any())
            {
                await roleManager.CreateAsync(new IdentityRole { Name = "Admin" });
                await roleManager.CreateAsync(new IdentityRole { Name = "HeadOfSchool" });
                await roleManager.CreateAsync(new IdentityRole { Name = "Teacher" });
                await roleManager.CreateAsync(new IdentityRole { Name = "Student" });
            }

            if (!dbcontext.Users.Any())
            {
                var user = new ApplicationUser("admin@picket.ocph23.tech") { Name = "Admin", Email = "admin@picket.ocph23.tech", EmailConfirmed = true };
                var result = await userManager.CreateAsync(user, "Password@123");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, "Admin");
                }
                else
                {
                    await userManager.DeleteAsync(user);
                }
            }

            if (!dbcontext.Departments.Any())
            {
               dbcontext.Departments.AddRange(new List<Department>
                {
                    new Department { Name = "Design Komunikas dan Visual" , Initial="DKV", Description=string.Empty },
                    new Department { Name = "Teknik Komunikasi dan Jaringan" , Initial="TKJ", Description=string.Empty },
                    new Department { Name = "Rekayasa Perangkat Lunak" , Initial="RPL", Description=string.Empty },
                    new Department { Name = "Kimia Industri" , Initial="KI", Description=string.Empty },
                });
               dbcontext.SaveChanges();
            }

            //if (!dbcontext.Students.Any())
            //{
            //   dbcontext.Students.AddRange(new List<Student>
            //    {
            //        new Student {NIS="11111111",  Name = "Valentinus Sipayung", DateOfBorn = new DateOnly(2003,3,3), Gender= SharedModel.Gender.Pria, PlaceOfBorn="Medan"  },
            //        new Student {NIS="11111112",  Name = "Revo Barus", DateOfBorn = new DateOnly(2003,3,3), Gender= SharedModel.Gender.Pria, PlaceOfBorn="Jayapura"  },
            //        new Student {NIS="11111113",  Name = "Michael Saja", DateOfBorn = new DateOnly(2003,3,3), Gender= SharedModel.Gender.Pria, PlaceOfBorn="Jayapura"  },
            //        new Student {NIS="11111114",  Name = "Andika Saja", DateOfBorn = new DateOnly(2003,3,3), Gender= SharedModel.Gender.Pria, PlaceOfBorn="Jayapura"  },
            //        new Student {NIS="11111115",  Name = "Duta Saja", DateOfBorn = new DateOnly(2003,3,3), Gender= SharedModel.Gender.Pria, PlaceOfBorn="Jayapura"  },
            //        new Student {NIS="11111116",  Name = "Angela Guru", DateOfBorn = new DateOnly(2003,3,3), Gender= SharedModel.Gender.Wanita, PlaceOfBorn="Soe"  },
            //    });
            //   dbcontext.SaveChanges();
            //}


            if (!dbcontext.Teachers.Any())
            {
               dbcontext.Teachers.AddRange(new List<Teacher>
                {
                    new Teacher {RegisterNumber="511111111",  Name = "Nasrullah", DateOfBorn = new DateOnly(2003,3,3), Gender= SharedModel.Gender.Pria, PlaceOfBorn="Medan"  },
                    new Teacher {RegisterNumber="511111112",  Name = "Juan Latuperisa", DateOfBorn = new DateOnly(2003,3,3), Gender= SharedModel.Gender.Pria, PlaceOfBorn="Jayapura"  },
                    new Teacher {RegisterNumber="511111113",  Name = "Yoseph Kungkung", DateOfBorn = new DateOnly(2003,3,3), Gender= SharedModel.Gender.Pria, PlaceOfBorn="Jayapura"  },
                    new Teacher {RegisterNumber="511111114",  Name = "Himawan", DateOfBorn = new DateOnly(2003,3,3), Gender= SharedModel.Gender.Pria, PlaceOfBorn="Jayapura"  },
                    new Teacher {RegisterNumber="511111115",  Name = "Videl Silak", DateOfBorn = new DateOnly(2003,3,3), Gender= SharedModel.Gender.Pria, PlaceOfBorn="Jayapura"  },
                    new Teacher {RegisterNumber="511111116",  Name = "Mira Cucu", DateOfBorn = new DateOnly(2003,3,3), Gender= SharedModel.Gender.Wanita, PlaceOfBorn="Soe"  },
                });
               dbcontext.SaveChanges();
            }


        }
    }

}
