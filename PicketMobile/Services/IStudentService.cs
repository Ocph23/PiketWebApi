using SharedModel.Models;
using SharedModel.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PicketMobile.Services
{
    public interface IStudentService
    {
        Task<IEnumerable<Student>> SearchStudent(string searchText);


    }


    public class StudentService : IStudentService
    {
        public async Task<IEnumerable<Student>> SearchStudent(string searchText)
        {
            try
            {
                using var db = new RestClient();
                HttpResponseMessage response = await db.GetAsync($"api/student/search/{searchText}");
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.GetResultAsync<IEnumerable<Student>>();
                    return result!=null?result:Enumerable.Empty<Student>();
                }
                throw new SystemException(await db.Error(response));
            }
            catch (Exception ex)
            {
                throw new SystemException(ex.Message);
            }
        }
    }
}
