using PicketMobile.Models;
using SharedModel.Requests;
using SharedModel.Responses;
using System.Net.Http.Json;
using System.Text.Json;

namespace PicketMobile.Services
{

    public interface IPicketService
    {
        Task<PaginationResponse<PicketResponse>> Get(PaginationRequest req);
        Task<PicketResponse> GetById(int id);
        Task<PicketResponse> GetPicketToday();
        Task<PicketModel> Create(PicketModel model);
        Task<LateAndGoHomeEarlyResponse> AddLateandEarly(StudentToLateAndEarlyRequest model);
        Task<bool> DeleteToLate(int studentTolateId);
        Task<bool> DeleteToComeHomeEarly(int studentGoHomeErly);
        Task<bool> Put(int id, PicketRequest model);
    }

    public class PicketService : IPicketService
    {
        private static PicketResponse picket;
        public async Task<PicketModel> Create(PicketModel model)
        {
            try
            {
                using var client = new RestClient();
                HttpResponseMessage response = await client.PostAsJsonAsync($"api/picket", model);
                if (response.IsSuccessStatusCode)
                {
                    var stringContent = await response.Content.ReadAsStringAsync();
                    var result = await response.GetResultAsync<PicketModel>();
                    if (result != null)
                        return result;
                }
                throw new SystemException(await client.Error(response));
            }
            catch (Exception ex)
            {
                throw new SystemException(ex.Message);
            }
        }
        public async Task<PicketResponse> GetPicketToday()
        {
            try
            {

                if (picket != null && DateOnly.FromDateTime(picket.CreateAt) == DateOnly.FromDateTime(DateTime.Now))
                    return picket;

                using var client = new RestClient();
                HttpResponseMessage response = await client.GetAsync($"api/picket");
                if (response.IsSuccessStatusCode)
                {
                    picket = await response.GetResultAsync<PicketResponse>();
                    return picket;

                }
                throw new SystemException(await client.Error(response));
            }
            catch (Exception ex)
            {
                throw new SystemException(ex.Message);
            }
        }
        public async Task<bool> DeleteToComeHomeEarly(int studentGoHomeErly)
        {
            try
            {
                using var client = new RestClient();
                var response = await client.DeleteAsync($"/picket/removehomeearly");
                if (response.IsSuccessStatusCode)
                    return true;
                return false;
            }
            catch (Exception ex)
            {
                throw new SystemException(ex.Message);
            }
        }

        public async Task<bool> DeleteToLate(int studentTolateId)
        {
            try
            {
                using var client = new RestClient();
                var response = await client.DeleteAsync($"/picket/removelate");
                if (response.IsSuccessStatusCode)
                    return true;
                return false;
            }
            catch (Exception ex)
            {
                throw new SystemException(ex.Message);
            }
        }

        public async Task<LateAndGoHomeEarlyResponse> AddLateandEarly(StudentToLateAndEarlyRequest model)
        {
            try
            {
                using var client = new RestClient();

                var data = client.GenerateHttpContent(model);


                HttpResponseMessage response = await client.PostAsJsonAsync($"/api/picket/lateandearly", model);
                if (response.IsSuccessStatusCode)
                {
                    var stringContent = await response.Content.ReadAsStringAsync();
                    var result = JsonSerializer.Deserialize<LateAndGoHomeEarlyResponse>(stringContent,Helper.JsonOption);
                    if (result != null)
                    {
                        if(picket !=null && picket.StudentsLateAndComeHomeEarly != null)
                        {
                            picket.StudentsLateAndComeHomeEarly.Add(result);
                        }
                        return result; 
                    }
                }
                throw new SystemException(await client.Error(response));
            }
            catch (Exception ex)
            {
                throw new SystemException(ex.Message);
            }
        }

        public async Task<bool> Put(int id, PicketRequest model)
        {
            try
            {
                using var client = new RestClient();
                var x = client.GenerateHttpContent(model);
                HttpResponseMessage response = await client.PutAsJsonAsync($"api/picket/{id}", model);
                if (response.IsSuccessStatusCode)
                {
                    var stringContent = await response.Content.ReadAsStringAsync();
                    var result = await response.GetResultAsync<bool>();
                    if (result)
                        return result;
                }
                throw new SystemException(await client.Error(response));
            }
            catch (Exception ex)
            {
                throw new SystemException(ex.Message);
            }
        }

        public async Task<PaginationResponse<PicketResponse>> Get(PaginationRequest req)
        {
            try
            {
                using var client = new RestClient();
                HttpResponseMessage response = await client.PostAsJsonAsync($"api/picket/paginate", req);
                if (response.IsSuccessStatusCode)
                {
                    var stringContent = await response.Content.ReadAsStringAsync();
                    var result = await response.GetResultAsync<PaginationResponse<PicketResponse>>();
                    if (result != null)
                        return result;
                }
                throw new SystemException(await client.Error(response));
            }
            catch (Exception ex)
            {
                throw new SystemException(ex.Message);
            }
        }

     

        public async Task<PicketResponse> GetById(int id)
        {
            try
            {
                using var client = new RestClient();
                HttpResponseMessage response = await client.GetAsync($"api/picket/{id}");
                if (response.IsSuccessStatusCode)
                {
                    picket = await response.GetResultAsync<PicketResponse>();
                    return picket;

                }
                throw new SystemException(await client.Error(response));
            }
            catch (Exception ex)
            {
                throw new SystemException(ex.Message);
            }
        }
    }
}
