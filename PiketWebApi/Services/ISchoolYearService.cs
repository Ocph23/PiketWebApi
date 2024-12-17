using ErrorOr;
using FluentValidation.Results;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PiketWebApi.Data;
using SharedModel.Models;

namespace PiketWebApi.Services
{
    public interface ISchoolYearService
    {
        Task<ErrorOr<SchoolYear>> GetSchoolYearById(int id);
        Task<ErrorOr<SchoolYear>> GetActiveSchoolYear();
        Task<ErrorOr<bool>> DeleteSchoolYear(int id);
        Task<ErrorOr<bool>> PutSchoolYear(int id, SchoolYear teacher);
        Task<ErrorOr<SchoolYear>> PostSchoolYear(SchoolYear model);
        Task<ErrorOr<IEnumerable<SchoolYear>>> GetAllSchoolYear();

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

        public async Task<ErrorOr<SchoolYear>> GetSchoolYearById(int id)
        {
            try
            {
                var result = dbContext.SchoolYears.SingleOrDefault(x => x.Id == id);
                if (result == null)
                    return Error.NotFound("Data tidak ditemukan");
                return await Task.FromResult(result);
            }
            catch (Exception)
            {
                return Error.Conflict();
            }
        }
        public async Task<ErrorOr<SchoolYear>> GetActiveSchoolYear()
        {
            try
            {
                var result = dbContext.SchoolYears.SingleOrDefault(x => x.Actived);
                if (result == null)
                    return Error.NotFound("NotFound","Tahun Ajaran aktif tidak ditemukan");
                return await Task.FromResult(result);
            }
            catch (Exception)
            {
                return Error.Conflict();
            }
        }

        public async Task<ErrorOr<bool>> DeleteSchoolYear(int id)
        {
            try
            {
                var result = dbContext.SchoolYears.SingleOrDefault(x => x.Id == id);
                if (result != null)
                {
                    dbContext.Remove(result);
                    dbContext.SaveChanges();
                    return await Task.FromResult(true);
                }
                return Error.NotFound("Data tidak ditemukan");
            }
            catch (Exception)
            {
                return Error.Conflict();
            }
        }

        public async Task<ErrorOr<bool>> PutSchoolYear(int id, SchoolYear model)
        {
            try
            {
                var validator = new Validators.SchoolYearValidator();
                ValidationResult validateResult = validator.Validate(model);
                if (!validateResult.IsValid)
                {
                    return validateResult.GetErrors();
                }

                var isExists = dbContext.SchoolYears.Any(x => x.Id != id && x.Year == model.Year && model.Semester == model.Semester);
                if (isExists)
                {
                    return Error.Conflict($"Data tahun ajaran {model.Year} semester {model.Year} sudah ada");
                }

                var result = dbContext.SchoolYears.SingleOrDefault(x => x.Id == id);
                if (result != null)
                {
                    dbContext.Entry(result).CurrentValues.SetValues(model);
                    dbContext.SaveChanges();
                    return await Task.FromResult(true);
                }
                return Error.NotFound("Data tidak ditemukan");
            }
            catch (Exception)
            {
                return Error.Conflict();
            }
        }

        public async Task<ErrorOr<SchoolYear>> PostSchoolYear(SchoolYear model)
        {
            var trans = await dbContext.Database.BeginTransactionAsync();

            try
            {
                var validator = new Validators.SchoolYearValidator();
                ValidationResult validateResult = validator.Validate(model);
                if (!validateResult.IsValid)
                {
                    return validateResult.GetErrors();
                }

                var isExists = dbContext.SchoolYears.Any(x => x.Year == model.Year && x.Semester == model.Semester);
                if (isExists)
                {
                    return Error.Conflict("SchoolYear", $"Data tahun ajaran {model.Year} semester {model.Semester} sudah ada");
                }

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
                    return Error.Failure("BadRequest", $"Tahun Ajaran {model.Year} sudah ada");
                return Error.Conflict();
            }
        }

        public async Task<ErrorOr<IEnumerable<SchoolYear>>> GetAllSchoolYear()
        {
            try
            {
                return await Task.FromResult(dbContext.SchoolYears.ToList());
            }
            catch (Exception)
            {
                return Error.Conflict();
            }
        }

    }
}
