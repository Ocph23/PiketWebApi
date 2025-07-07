using ErrorOr;
using Microsoft.EntityFrameworkCore;
using PiketWebApi.Abstractions;
using PiketWebApi.Data;
using PiketWebApi.Services;
using SharedModel.Models;

namespace PiketWebApi.Api
{
    public static class DashboardApi
    {
        public static RouteGroupBuilder MapDashboardApi(this RouteGroupBuilder group)
        {
            group.MapGet("/", GetMainDashboard);
            return group.WithTags("dashboard").RequireAuthorization(); ;
        }

        private static async Task<IResult> GetMainDashboard(HttpContext context, IDashboardService dashboardService)
        {
            var result = await dashboardService.Get();
            return result.Match(items => Results.Ok(items), errors => Results.BadRequest(result.CreateProblemDetail(context)));
        }
    }
}
