using SharedModel.Models;

namespace SharedModel.Responses
{
    public record AuthenticateResponse(string UserName, string Email, IList<string> roles, string Token, Profile? Profile);
}