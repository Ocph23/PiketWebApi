using PicketMobile.Models;
using SharedModel.Models;
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
        Task<PicketModel> GetPicketToday();
        Task<PicketModel> Create(PicketModel model);
        Task<StudentToLate> PostToLate(StudentToLate model);
        Task<bool> DeleteToLate(int studentTolateId);

        Task<StudentComeHomeEarly> PostToComeHomeEarly(StudentComeHomeEarly model);
        Task<bool> DeleteToComeHomeEarly(int studentGoHomeErly);
        Task<bool> Put(int id, PicketModel model);
    }

    public class PicketService : IPicketService
    {
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
        public async Task<PicketModel> GetPicketToday()
        {
            try
            {
                using var client = new RestClient();
                HttpResponseMessage response = await client.GetAsync($"api/picket");
                if (response.IsSuccessStatusCode)
                {
                    List<ScheduleModel> schedules = new List<ScheduleModel>();
                    var result = await response.GetResultAsync<PicketModel>();
                    return result;

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


        public async Task<StudentComeHomeEarly> PostToComeHomeEarly(StudentComeHomeEarly model)
        {
            try
            {
                using var client = new RestClient();
                HttpResponseMessage response = await client.PostAsJsonAsync($"/picket/createsoearly", model);
                if (response.IsSuccessStatusCode)
                {
                    var stringContent = await response.Content.ReadAsStringAsync();
                    var result = JsonSerializer.Deserialize<StudentComeHomeEarly>(stringContent);
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

        public async Task<StudentToLate> PostToLate(StudentToLate model)
        {
            try
            {
                using var client = new RestClient();
                HttpResponseMessage response = await client.PostAsJsonAsync($"/picket/createlate", model);
                if (response.IsSuccessStatusCode)
                {
                    var stringContent = await response.Content.ReadAsStringAsync();
                    var result = JsonSerializer.Deserialize<StudentToLate>(stringContent);
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

        public async Task<bool> Put(int id, PicketModel model)
        {
            try
            {
                using var client = new RestClient();
                HttpResponseMessage response = await client.PutAsJsonAsync($"api/picket/{id}", model);
                if (response.IsSuccessStatusCode)
                {
                    var stringContent = await response.Content.ReadAsStringAsync();
                    var result = await response.GetResultAsync<bool>();
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
    }
}
