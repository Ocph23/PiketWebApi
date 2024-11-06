using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PiketWebApi.Data;
using SharedModel.Models;
using System.Security.Claims;

namespace PiketWebApi
{
    public static class AppExtention
    {
        public static async Task<(bool,Teacher?)> IsTeacherPicket(this IHttpContextAccessor http, UserManager<ApplicationUser> userManager, ApplicationDbContext dbContext)
        {
            try
            {
                var userClaim = http.HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier);
                var id = userClaim?.Value.ToString();
                var user = await userManager.FindByEmailAsync(id);
                if (user == null)
                {
                    throw new UnauthorizedAccessException();
                }
                var teacher = dbContext.Teachers.SingleOrDefault(x => x.UserId == user.Id);
                if (teacher == null)
                {
                    throw new UnauthorizedAccessException();
                }
                var schedule = dbContext.Schedules.Where(x => x.DayOfWeek == DateTime.Now.DayOfWeek && x.Teacher.Id == teacher.Id).Include(x => x.Teacher);
                return (true, teacher);
            }
            catch (Exception)
            {
                return (false, null);
            }
        }
    }
}
