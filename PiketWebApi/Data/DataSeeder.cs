using Microsoft.EntityFrameworkCore;

namespace PiketWebApi.Data
{
    public static class DataSeeder
    {
        public static void SeedData(WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            context.Database.Migrate(); // Apply pending migrations

            if (!context.Departments.Any())
            {
                context.Departments.AddRange(new List<Department>
                {
                    new Department { Name = "Design Komunikas dan Visual" , Initial="DKV", Description=string.Empty },
                    new Department { Name = "Teknik Komunikasi dan Jaringan" , Initial="TKJ", Description=string.Empty },
                    new Department { Name = "Rekayasa Perangkat Lunak" , Initial="RPL", Description=string.Empty },
                    new Department { Name = "Kimia Industri" , Initial="KI", Description=string.Empty },
                });
                context.SaveChanges();
            }

            if (!context.Students.Any())
            {
                context.Students.AddRange(new List<Student>
                {
                    new Student {NIS="11111111",  Name = "Valentinus Sipayung", DateOfBorn = new DateOnly(2003,3,3), Gender= SharedModel.Gender.Pria, PlaceOfBorn="Medan"  },
                    new Student {NIS="11111112",  Name = "Revo Barus", DateOfBorn = new DateOnly(2003,3,3), Gender= SharedModel.Gender.Pria, PlaceOfBorn="Jayapura"  },
                    new Student {NIS="11111113",  Name = "Michael Saja", DateOfBorn = new DateOnly(2003,3,3), Gender= SharedModel.Gender.Pria, PlaceOfBorn="Jayapura"  },
                    new Student {NIS="11111114",  Name = "Andika Saja", DateOfBorn = new DateOnly(2003,3,3), Gender= SharedModel.Gender.Pria, PlaceOfBorn="Jayapura"  },
                    new Student {NIS="11111115",  Name = "Duta Saja", DateOfBorn = new DateOnly(2003,3,3), Gender= SharedModel.Gender.Pria, PlaceOfBorn="Jayapura"  },
                    new Student {NIS="11111116",  Name = "Angela Guru", DateOfBorn = new DateOnly(2003,3,3), Gender= SharedModel.Gender.Wanita, PlaceOfBorn="Soe"  },
                });
                context.SaveChanges();
            }


            if (!context.Teachers.Any())
            {
                context.Teachers.AddRange(new List<Teacher>
                {
                    new Teacher {RegisterNumber="511111111",  Name = "Nasrullah", DateOfBorn = new DateOnly(2003,3,3), Gender= SharedModel.Gender.Pria, PlaceOfBorn="Medan"  },
                    new Teacher {RegisterNumber="511111112",  Name = "Juan Latuperisa", DateOfBorn = new DateOnly(2003,3,3), Gender= SharedModel.Gender.Pria, PlaceOfBorn="Jayapura"  },
                    new Teacher {RegisterNumber="511111113",  Name = "Yoseph Kungkung", DateOfBorn = new DateOnly(2003,3,3), Gender= SharedModel.Gender.Pria, PlaceOfBorn="Jayapura"  },
                    new Teacher {RegisterNumber="511111114",  Name = "Himawan", DateOfBorn = new DateOnly(2003,3,3), Gender= SharedModel.Gender.Pria, PlaceOfBorn="Jayapura"  },
                    new Teacher {RegisterNumber="511111115",  Name = "Videl Silak", DateOfBorn = new DateOnly(2003,3,3), Gender= SharedModel.Gender.Pria, PlaceOfBorn="Jayapura"  },
                    new Teacher {RegisterNumber="511111116",  Name = "Mira Cucu", DateOfBorn = new DateOnly(2003,3,3), Gender= SharedModel.Gender.Wanita, PlaceOfBorn="Soe"  },
                });
                context.SaveChanges();
            }

           
        }
    }

}
