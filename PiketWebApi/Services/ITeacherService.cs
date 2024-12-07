using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PiketWebApi.Data;
using SharedModel.Models;
using SharedModel.Requests;
using SharedModel.Responses;
using System.Net.Sockets;
using System.Security.Claims;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace PiketWebApi.Services
{
    public interface ITeacherService
    {
        Task<bool> Delete(int id);
        Task<bool> Put(int id, Teacher model);
        Task<Teacher> Post(Teacher model);
        Task<IEnumerable<Teacher>> Get();
        Task<Teacher?> GetById(int id);

    }

    public class TeacherService : ITeacherService
    {
        private readonly IHttpContextAccessor http;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly ApplicationDbContext dbContext;
        private static Picket picketToday;

        public TeacherService(IHttpContextAccessor _http, UserManager<ApplicationUser> _userManager,
            ApplicationDbContext _dbContext)
        {
            http = _http;
            userManager = _userManager;
            dbContext = _dbContext;
        }

        public Task<bool> Delete(int id)
        {
            try
            {
                var result = dbContext.Teachers.SingleOrDefault(x => x.Id == id);
                if (result != null)
                {
                    dbContext.Remove(result);
                    dbContext.SaveChanges();
                }
                return Task.FromResult(true);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public Task<bool> Put(int id, Teacher model)
        {
            try
            {
                var result = dbContext.Teachers.SingleOrDefault(x => x.Id == id);
                if (result != null)
                {
                    dbContext.Entry(result).CurrentValues.SetValues(model);
                    dbContext.SaveChanges();
                }
                return Task.FromResult(true);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<Teacher> Post(Teacher model)
        {
            var trans = dbContext.Database.BeginTransaction();
            try
            {
                if (!string.IsNullOrEmpty(model.Email))
                {
                    var user = new ApplicationUser { Email = model.Email, EmailConfirmed = true, Name = model.Name, UserName = model.Email };
                    var createResult = await userManager.CreateAsync(user, "Password@123");
                    if (createResult.Succeeded)
                    {
                        await userManager.AddToRoleAsync(user, "Teacher");
                        model.UserId = user.Id;
                    }
                    else
                    {
                        throw new SystemException("User Gagal Dibuat !");
                    }
                }

                var result = dbContext.Teachers.Add(model);
                dbContext.SaveChanges();
                trans.Commit();
                return model;
            }
            catch (Exception ex)
            {
                trans.Rollback();
                throw;
            }
        }

        public Task<IEnumerable<Teacher>> Get()
        {
            try
            {
                return Task.FromResult(dbContext.Teachers.AsEnumerable());
            }
            catch (Exception)
            {
                throw;
            }
        }

        public Task<Teacher?> GetById(int id)
        {
            try
            {
                return Task.FromResult(dbContext.Teachers.SingleOrDefault(x => x.Id == id));
            }
            catch (Exception)
            {
                throw;
            }
        }


    }
}
