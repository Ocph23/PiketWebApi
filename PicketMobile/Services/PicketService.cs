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
        Task<StudentToLate> PostToLate(StudentToLate model);
        Task<bool> DeleteToLate(int studentTolateId);

        Task<StudentComeHomeEarly> PostToComeHomeEarly(StudentComeHomeEarly model);
        Task<bool> DeleteToComeHomeEarly(int studentGoHomeErly);

        Task<IEnumerable<ScheduleModel>> GetScheduleActive();


    }

    public class PicketService : IPicketService
    {
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

        public async Task<IEnumerable<ScheduleModel>> GetScheduleActive()
        {
            try
            {
                using var db = new RestClient();
                HttpResponseMessage response = await db.GetAsync($"api/schedule/active");
                if (response.IsSuccessStatusCode)
                {
                    List<ScheduleModel> schedules = new List<ScheduleModel>();
                    var result = await response.GetResult<IEnumerable<ScheduleResponse>>();
                    if (result != null)
                    {
                        var group = result.GroupBy(x => x.DayOfWeek);
                        foreach (var item in group)
                        {
                            schedules.Add(new ScheduleModel
                            {
                                Day = item.Key.ToString(),
                                Members = item.ToList()
                            });
                        }
                    }
                    return schedules;

                }
                throw new SystemException(await db.Error(response));
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
