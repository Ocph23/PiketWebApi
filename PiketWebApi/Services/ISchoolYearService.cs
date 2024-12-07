using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PiketWebApi.Data;
using SharedModel.Models;

namespace PiketWebApi.Services
{
    public interface ISchoolYearService
    {
        public Task<SchoolYear?> GetSchoolYearById(int id);
        Task<SchoolYear?> GetActiveSchoolYear();
        Task<bool> DeleteSchoolYear(int id);
        Task<bool> PutSchoolYear(int id, SchoolYear teacher);
        Task<SchoolYear> PostSchoolYear(SchoolYear model);
        Task<IEnumerable<SchoolYear>> GetAllSchoolYear();

    }
    public class SchoolYearService : ISchoolYearService
    {
        public readonly IHttpContextAccessor http;
        public readonly UserManager<ApplicationUser> userManager;
        public readonly ApplicationDbContext dbContext;

        public SchoolYearService(
            IHttpContextAccessor _http,
            UserManager<ApplicationUser> _userManager,
            ApplicationDbContext _dbContext)
        {
            http = _http;
            userManager = _userManager;
            dbContext = _dbContext;
        }

        public Task<SchoolYear?> GetSchoolYearById(int id)
        {
            try
            {
                var result = dbContext.SchoolYears.SingleOrDefault(x => x.Id == id);
                return Task.FromResult(result);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public Task<SchoolYear?> GetActiveSchoolYear()
        {
            try
            {
                var result = dbContext.SchoolYears.SingleOrDefault(x => x.Actived);
                return Task.FromResult(result);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public Task<bool> DeleteSchoolYear(int id)
        {
            try
            {
                var result = dbContext.SchoolYears.SingleOrDefault(x => x.Id == id);
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

        public Task<bool> PutSchoolYear(int id, SchoolYear teacher)
        {
            try
            {
                var result = dbContext.SchoolYears.SingleOrDefault(x => x.Id == id);
                if (result != null)
                {
                    dbContext.Entry(result).CurrentValues.SetValues(teacher);
                    dbContext.SaveChanges();
                }
                return Task.FromResult(true);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<SchoolYear> PostSchoolYear(SchoolYear model)
        {
            var trans = await dbContext.Database.BeginTransactionAsync();

            try
            {
                var activeData = dbContext.SchoolYears.Where(x => x.Actived);
                await activeData.ForEachAsync((x) => x.Actived = false);
                model.Actived = true;
                dbContext.SchoolYears.Add(model);
                dbContext.SaveChanges();
                await trans.CommitAsync();
                return model;
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null && ex.InnerException.Message.Contains("duplicate"))
                    throw new SystemException($"Tahun Ajaran {model.Year} sudah ada");
                throw;
            }
        }

        public Task<IEnumerable<SchoolYear>> GetAllSchoolYear()
        {
            try
            {
                return Task.FromResult(dbContext.SchoolYears.AsEnumerable());
            }
            catch (Exception)
            {
                throw;
            }
        }

    }
}
