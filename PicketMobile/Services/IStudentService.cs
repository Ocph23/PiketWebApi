using SharedModel.Responses;

namespace PicketMobile.Services
{
    public interface IStudentService
    {
        Task<IEnumerable<StudentResponse>> SearchStudent(string searchText);


    }


    public class StudentService : IStudentService
    {
        public async Task<IEnumerable<StudentResponse>> SearchStudent(string searchText)
        {
            try
            {
                using var db = new RestClient();
                HttpResponseMessage response = await db.GetAsync($"api/student/search/{searchText}");
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.GetResultAsync<IEnumerable<StudentResponse>>();
                    return result!=null?result:Enumerable.Empty<StudentResponse>();
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
