using PicketMobile.Models;
using SharedModel.Models;
using SharedModel.Requests;
using SharedModel.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace PicketMobile.Services
{

    public interface IPicketService
    {
        Task<PicketResponse> GetPicketToday();
        Task<PicketModel> Create(PicketModel model);
        Task<StudentToLateAndHomeEarlyModel> PostToLate(StudentToLateAndEarlyRequest model);
        Task<bool> DeleteToLate(int studentTolateId);

        Task<StudentToLateAndHomeEarlyModel> PostToComeHomeEarly(StudentToLateAndEarlyRequest model);
        Task<bool> DeleteToComeHomeEarly(int studentGoHomeErly);
        Task<bool> Put(int id, PicketModel model);
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
                    List<ScheduleModel> schedules = new List<ScheduleModel>();
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


        public async Task<StudentToLateAndHomeEarlyModel> PostToComeHomeEarly(StudentToLateAndEarlyRequest model)
        {
            try
            {
                using var client = new RestClient();
                var content = client.GenerateHttpContent(model);
                HttpResponseMessage response = await client.PostAsJsonAsync($"/api/picket/early", model);
                if (response.IsSuccessStatusCode)
                {
                    var stringContent = await response.Content.ReadAsStringAsync();
                    var result = JsonSerializer.Deserialize<StudentToLateAndHomeEarlyModel>(stringContent);
                    if (result != null)
                        return result;

                }
                throw new SystemException("Maaf Terjadi Kesalahan, Coba Ulangi Lagi");
            }
            catch (Exception ex)
            {
                throw new SystemException(ex.Message);
            }
        }

        public async Task<StudentToLateAndHomeEarlyModel> PostToLate(StudentToLateAndEarlyRequest model)
        {
            try
            {
                using var client = new RestClient();

                var data = client.GenerateHttpContent(model);


                HttpResponseMessage response = await client.PostAsJsonAsync($"/api/picket/late", model);
                if (response.IsSuccessStatusCode)
                {
                    var stringContent = await response.Content.ReadAsStringAsync();
                    var result = JsonSerializer.Deserialize<StudentToLateAndHomeEarlyModel>(stringContent);
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

        public async Task<bool> Put(int id, PicketModel model)
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
    }
}
