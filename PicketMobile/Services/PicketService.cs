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
        Task<Picket> GetPicketToday();
        Task<Picket> Create(Picket model);
        Task<StudentToLate> PostToLate(StudentToLate model);
        Task<bool> DeleteToLate(int studentTolateId);

        Task<StudentComeHomeEarly> PostToComeHomeEarly(StudentComeHomeEarly model);
        Task<bool> DeleteToComeHomeEarly(int studentGoHomeErly);



    }

    public class PicketService : IPicketService
    {
        public async Task<Picket> Create(Picket model)
        {
            try
            {
                using var db = new RestClient();
                HttpResponseMessage response = await db.PostAsJsonAsync($"/picket/create", model);
                if (response.IsSuccessStatusCode)
                {
                    var stringContent = await response.Content.ReadAsStringAsync();
                    var result = await response.GetResultAsync<Picket>();
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
        public async Task<Picket> GetPicketToday()
        {
            try
            {
                using var db = new RestClient();
                HttpResponseMessage response = await db.GetAsync($"api/picket");
                if (response.IsSuccessStatusCode)
                {
                    List<ScheduleModel> schedules = new List<ScheduleModel>();
                    var result = await response.GetResultAsync<Picket>();
                    return result;

                }
                throw new SystemException(await db.Error(response));
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
                using var db = new RestClient();
                var response = await db.DeleteAsync($"/picket/removehomeearly");
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
                using var db = new RestClient();
                var response = await db.DeleteAsync($"/picket/removelate");
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
                using var db = new RestClient();
                HttpResponseMessage response = await db.PostAsJsonAsync($"/picket/createsoearly", model);
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
                using var db = new RestClient();
                HttpResponseMessage response = await db.PostAsJsonAsync($"/picket/createlate", model);
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
    }
}
