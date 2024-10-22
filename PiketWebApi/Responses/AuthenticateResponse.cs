namespace PiketWebApi.Responses
{
    public record AuthenticateResponse(string UserName, string Email, IList<string> roles, string Token);
}