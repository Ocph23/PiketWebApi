using ErrorOr;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PiketWebApi.Data;

namespace PiketWebApi.Services
{
    public interface IDepartmentService
    {
        Task<ErrorOr<IEnumerable<Department>>> GetAllAsync();
        Task<ErrorOr<Department>> GetByIdAsync(int id);
        Task<ErrorOr<bool>> DeleteAsync(int id);
        Task<ErrorOr<bool>> PutAsync(int id, Department model);
        Task<ErrorOr<Department>> PostAsync(Department model);
    }

    public class DepartmentService : IDepartmentService
    {
        private readonly IHttpContextAccessor http;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly ApplicationDbContext dbContext;
        private readonly ISchoolYearService schoolYearService;
        private static Picket picketToday;

        public DepartmentService( UserManager<ApplicationUser> _userManager,
            ApplicationDbContext _dbContext)
        {
            userManager = _userManager;
            dbContext = _dbContext;
        }

        public async Task<ErrorOr<bool>> DeleteAsync(int id)
        {
            try
            {
                var result = dbContext.Departments.SingleOrDefault(x => x.Id == id);
                if (result == null)
                    return Error.NotFound("NotFound", "Data jurusan tidak ditemukan.");
                dbContext.Remove(result);
                dbContext.SaveChanges();
                return await Task.FromResult(true);
            }
            catch (Exception)
            {
                return Error.Conflict();
            }
        }

        public async Task<ErrorOr<bool>> PutAsync(int id, Department model)
        {
            try
            {
                var result = dbContext.Departments.SingleOrDefault(x => x.Id == id);
                if (result == null)
                {
                    return Error.NotFound("NotFound", "Data jurusan tidak ditemukan.");
                }
                dbContext.Entry(result).CurrentValues.SetValues(model);
                dbContext.SaveChanges();
                return await Task.FromResult(true);
            }
            catch (Exception)
            {
                return Error.Conflict();
            }
        }

        public async Task<ErrorOr<Department>> PostAsync(Department model)
        {

            try
            {
                var validator = new Validators.DepartmentValidator();
                var validateResult = validator.Validate(model);
                if (!validateResult.IsValid)
                    return validateResult.GetErrors();  

                var result = dbContext.Departments.Add(model);
                dbContext.SaveChanges();
                return await Task.FromResult(model);
            }
            catch (Exception)
            {
                return Error.Conflict();
            }
        }

        public async Task<ErrorOr<IEnumerable<Department>>> GetAllAsync()
        {
            try
            {
                return await Task.FromResult(dbContext.Departments.ToList());
            }
            catch (Exception)
            {
                return Error.Conflict();
            }
        }

        public async Task<ErrorOr<Department>> GetByIdAsync(int id)
        {
            try
            {
                var result = dbContext.Departments.SingleOrDefault(x => x.Id == id);
                return await Task.FromResult(result);
            }
            catch (Exception)
            {
                return Error.Conflict();
            }
        }


    }
}
