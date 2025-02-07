using System;
using System.Collections.Generic;
using ErrorOr;
using Microsoft.EntityFrameworkCore;
using PiketWebApi.Data;
using SharedModel.Models;

namespace PiketWebApi.Services;

public interface IReportService
{
    Task<ErrorOr<IEnumerable<PicketReportModel>>> GetReportForAMount(int mount, int year);
}


public class ReportService : IReportService
{
    private readonly ApplicationDbContext _dbContext;

    public ReportService(ApplicationDbContext dbContext)
    {
        this._dbContext = dbContext;
    }

    public async Task<ErrorOr<IEnumerable<PicketReportModel>>> GetReportForAMount(int mount, int year)
    {
        try
        {
            var result = from x in _dbContext.Picket
                .Where(x => x.Date.Month == mount && x.Date.Year == year)
                .Include(x => x.CreatedBy)
                .Include(x => x.LateAndComeHomeEarly)
                         select new PicketReportModel(x.Id, x.Date, x.Weather,
                         x.StartAt, x.EndAt, x.CreatedBy.Name, x.CreatedBy.RegisterNumber,x.CreateAt,
                          x.LateAndComeHomeEarly.Count(x => x.LateAndGoHomeEarlyStatus == SharedModel.LateAndGoHomeEarlyAttendanceStatus.Terlambat),
                         x.LateAndComeHomeEarly.Count(x => x.LateAndGoHomeEarlyStatus == SharedModel.LateAndGoHomeEarlyAttendanceStatus.Pulang));

            return await Task.FromResult(result.ToList());
        }
        catch (System.Exception)
        {
            return
             Error.Conflict();
        }
    }
}




