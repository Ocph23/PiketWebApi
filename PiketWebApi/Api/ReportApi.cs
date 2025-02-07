using System;
using PiketWebApi.Services;

namespace PiketWebApi.Api;

public static class ReportApi
{


    public static RouteGroupBuilder MapReportApi(this RouteGroupBuilder group)
    {
        group.MapGet("/picket/month/{month}/{year}", GetPicketForAMount);

        return group.WithTags("report").RequireAuthorization();
    }

    private static async Task<IResult> GetPicketForAMount(HttpContext context, IReportService reportService, int month, int year)
    {
        var result = await reportService.GetReportForAMount(month, year);
        return result.Match(items => Results.Ok(items), errors => Results.BadRequest(result.CreateProblemDetail(context)));
    }
}
