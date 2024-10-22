
namespace PiketWebApi.Api
{
    public static class TeacherApi
    {

        public static RouteGroupBuilder MapTeacherApi (this RouteGroupBuilder group)
        {
            group.MapGet("/", () => { return "OK"; }).RequireAuthorization();
            return group.WithTags("teacher");
        }

        private static async Task GetAllTeacher(HttpContext context)
        {
            throw new NotImplementedException();
        }
    }
}
