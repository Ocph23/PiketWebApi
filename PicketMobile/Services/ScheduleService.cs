﻿using PicketMobile.Models;
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

    public interface IScheduleService
    {
        Task<IEnumerable<ScheduleModel>> GetScheduleActive();
        Task<bool> IamPicket();
    }

    public class ScheduleService : IScheduleService
    {
        static List<ScheduleModel> schedules = new List<ScheduleModel>();



        public async Task<IEnumerable<ScheduleModel>> GetScheduleActive()
        {
            try
            {
                if (schedules.Count <= 0)
                {
                    using var db = new RestClient();
                    HttpResponseMessage response = await db.GetAsync($"api/schedule/active");
                    if (response.IsSuccessStatusCode)
                    {
                        schedules.Clear();
                        var result = await response.GetResultAsync<IEnumerable<ScheduleResponse>>();
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
                else
                    return schedules;

            }
            catch (Exception ex)
            {
                throw new SystemException(ex.Message);
            }
        }

        public async Task<bool> IamPicket()
        {
            try
            {
                var schedules = await GetScheduleActive();
                var profile = new Teacher() { Id=1};

                var myschedulu = schedules.Where(x => x.Members.Any(z => z.TeacherId == profile.Id)).FirstOrDefault();

                if (myschedulu!=null && myschedulu.Day== DateTime.Now.DayOfWeek.ToString())
                    return true;
                return false;

            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}