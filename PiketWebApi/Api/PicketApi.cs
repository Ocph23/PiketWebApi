using PiketWebApi.Services;
using SharedModel.Requests;
using SharedModel.Responses;

namespace PiketWebApi.Api
{
    public static class PickerApi
    {
        public static RouteGroupBuilder MapPickerApi(this RouteGroupBuilder group)
        {
            group.MapGet("/", GetPickerToday);
            group.MapPost("/paginate", GetAllWithPanitate);
            group.MapGet("/{id}", GetById);
            group.MapPost("/", PostPicket);
            group.MapPut("/{id}", PutPicket);
            group.MapPost("/lateandearly", AddLateandearly);
            group.MapDelete("/lateandearly/{id}", RemoveLateandearly);
            group.MapPost("/deilyjournal", AddDailyJournal);
            group.MapDelete("/deilyjournal/{id}", RemoveDailyJournal);
            return group.WithTags("picket").RequireAuthorization(); ;
        }

        private static async Task<IResult> RemoveDailyJournal(HttpContext context, IPicketService picketService, int id)
        {
           var result = await picketService.RemoveDailyJournal(id);
            return result.Match(items => Results.Ok(items), errors => Results.BadRequest(result.CreateProblemDetail(context)));
        }

        private static async Task<IResult> AddDailyJournal(HttpContext context, IPicketService picketService, DailyJournalRequest model)
        {
            var result = await picketService.AddDailyJournal(model);
            return result.Match(items => Results.Ok(items), errors => Results.BadRequest(result.CreateProblemDetail(context)));
        }
        
        private static async Task<IResult> GetAllWithPanitate(HttpContext context, IPicketService picketService, PaginationRequest req)
        {
            var result = await picketService.GetWithPaginate(req);
            return result.Match(items => Results.Ok(items), errors => Results.BadRequest(result.CreateProblemDetail(context)));
        }

        private static async Task<IResult> GetById(HttpContext context, IPicketService picketService, int id)
        {
            var result = await picketService.GetById(id);
            return result.Match(items => Results.Ok(items), errors => Results.BadRequest(result.CreateProblemDetail(context)));
        }

        private static async Task<IResult> PutPicket(HttpContext context, IPicketService picketService, int id, PicketRequest picket)
        {
            var result = await picketService.UpdatePicket(id, picket);
            return result.Match(items => Results.Ok(items), errors => Results.BadRequest(result.CreateProblemDetail(context)));
        }

        private static async Task<IResult> GetPickerToday(HttpContext context, IPicketService picketService)
        {
            var result = await picketService.GetPicketToday();
            return result.Match(items => Results.Ok(items), errors => Results.BadRequest(result.CreateProblemDetail(context)));

        }


        private static async Task<IResult> RemoveLateandearly(HttpContext context, IPicketService picketService, int id)
        {
            var result = await picketService.RemoveStudentToLateComeHomeSoEarly(id);
            return result.Match(items => Results.Ok(items), errors => Results.BadRequest(result.CreateProblemDetail(context)));
        }

     
        private static async Task<IResult> AddLateandearly(HttpContext context, IPicketService picketService, StudentToLateAndEarlyRequest model)
        {
            var result = await picketService.AddStudentToLateComeHomeSoEarly(model);
            return result.Match(items => Results.Ok(items), errors => Results.BadRequest(result.CreateProblemDetail(context)));

        }

        private static async Task<IResult> PostPicket(HttpContext context, IPicketService picketService)
        {
            var result = await picketService.CreateNewPicket();
            return result.Match(items => Results.Ok(items),  errors => result.GetErrorResult(context));
        }

    }
}
