using ErrorOr;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using PiketWebApi.Data;
using System.Linq;

namespace PiketWebApi.Services
{
    public interface ITeacherService
    {
        Task<ErrorOr<bool>> DeleteAsync(int id);
        Task<ErrorOr<bool>> PutAsync(int id, Teacher model);
        Task<ErrorOr<Teacher>> PostAsync(Teacher model);
        Task<ErrorOr<IEnumerable<Teacher>>> GetAsync();
        Task<ErrorOr<Teacher>> GetByIdAsync(int id);
        Task<ErrorOr<IEnumerable<Teacher>>> SearchTextAsync(string text);
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

        public async Task<ErrorOr<bool>> DeleteAsync(int id)
        {
            try
            {
                var result = dbContext.Teachers.SingleOrDefault(x => x.Id == id);
                if (result == null)
                    return Error.NotFound("Data guru tidak ditemukan.");


                dbContext.Remove(result);
                dbContext.SaveChanges();
                return await Task.FromResult(true);
            }
            catch (Exception)
            {
                return Error.Conflict();
            }
        }

        public async Task<ErrorOr<bool>> PutAsync(int id, Teacher model)
        {
            var trans = dbContext.Database.BeginTransaction();
            try
            {
                var validator = new Validators.TeacherValidator();
                var validatorResult = validator.Validate(model);
                if (!validatorResult.IsValid)
                    return validatorResult.GetErrors();
                var result = dbContext.Teachers.SingleOrDefault(x => x.Id == id);
                if (result == null)
                {
                    return Error.NotFound("Data guru tidak ditemukan.");
                }

                if (!string.IsNullOrEmpty(model.Email) && string.IsNullOrEmpty(result.Email))
                {
                    var userResult = await Helper.CreateUser(userManager,
                        new ApplicationUser { Email = model.Email, EmailConfirmed = true, Name = model.Name, UserName = model.Email },
                        "Teacher");
                    if (userResult.IsError)
                    {
                        trans.Rollback();
                        return userResult.Errors;
                    }
                    model.UserId = userResult.Value.Id;
                }

                dbContext.Entry(result).CurrentValues.SetValues(model);
                dbContext.SaveChanges();
                trans.Commit();
                return await Task.FromResult(true);
            }
            catch (Exception)
            {
                trans.Rollback();
                return Error.Conflict();
            }
        }

        public async Task<ErrorOr<Teacher>> PostAsync(Teacher model)
        {
            var trans = dbContext.Database.BeginTransaction();
            try
            {
                if (!string.IsNullOrEmpty(model.Email))
                {
                    var userResult = await Helper.CreateUser(userManager,
                        new ApplicationUser { Email = model.Email, EmailConfirmed = true, Name = model.Name, UserName = model.Email },
                        "Teacher");
                    if (userResult.IsError)
                    {
                        trans.Rollback();
                        return userResult.Errors;
                    }
                    model.UserId = userResult.Value.Id;
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

        public async Task<ErrorOr<IEnumerable<Teacher>>> GetAsync()
        {
            try
            {
                return await Task.FromResult(dbContext.Teachers.ToList());
            }
            catch (Exception)
            {
                return Error.Conflict();
            }
        }

        public async Task<ErrorOr<Teacher>> GetByIdAsync(int id)
        {
            try
            {
                var result = dbContext.Teachers.SingleOrDefault(x => x.Id == id);
                if (result == null)
                    return Error.NotFound("NotFound", "Data siswa tidak ditemukan");
                return await Task.FromResult(result);
            }
            catch (Exception)
            {
                return Error.Conflict();
            }
        }

        public async Task<ErrorOr<IEnumerable<Teacher>>> SearchTextAsync(string searchtext)
        {
            try
            {
                var txtSearch = searchtext.ToLower();
                IEnumerable<Teacher> result = dbContext.Teachers.Where(x => x.Name.ToLower().Contains(txtSearch)
                || x.Email!.ToLower().Contains(txtSearch)
                || x.RegisterNumber!.ToLower().Contains(txtSearch)).ToList();

                return await Task.FromResult(result.ToList());
            }
            catch (Exception)
            {
                return Error.Conflict();
            }
        }
    }
}
