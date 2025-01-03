
namespace SharedModel.Responses;

public record AuthenticateResponse(string UserName, string Email, IList<string> roles, string Token, object? Profile);