using ErrorOr;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using PiketWebApi.Data;
using PiketWebApi.Services;
using SharedModel.Models;
using SharedModel.Responses;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace PiketWebApi.Api
{
    public static class AuthApi
    {

        public static RouteGroupBuilder MapAuthApi(this RouteGroupBuilder group)
        {
            group.MapPost("/login", LoginAction);
            group.MapPost("/register", RegisterAction);
            group.MapGet("/active", ActiveAccout).RequireAuthorization("admin_policy");
            group.MapGet("/setadmin/{userId}", SetAsAdmin);
            return group;
        }

        private static async Task<IResult> SetAsAdmin(HttpContext context, UserManager<ApplicationUser> userManager, string userId)
        {
            try
            {
                var user = await userManager.FindByIdAsync(userId);
                if (user != null)
                {
                    await userManager.AddToRoleAsync(user, "Admin");
                    return Results.Ok();
                }

                ErrorOr<bool> error = Error.Failure("Data user tidak ditemukan");
                return Results.BadRequest(error.CreateProblemDetail(context));
            }
            catch (Exception ex)
            {
                return Results.BadRequest(ex.Message);
            }
        }

        private static IResult ActiveAccout(HttpContext context)
        {
            try
            {
                return TypedResults.Ok();
            }
            catch (Exception)
            {
                return TypedResults.Unauthorized();
            }
        }

        private static async Task RegisterAction(HttpContext context)
        {
            throw new NotImplementedException();
        }

        private static async Task<IResult> LoginAction(
           SharedModel.Requests.LoginRequest request,
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager, IConfiguration _config,
            ApplicationDbContext dbContext
            )
        {
            try
            {
                AppSettings _appSettings = new AppSettings();
                _config.GetSection("AppSettings").Bind(_appSettings);
                var result = await signInManager.PasswordSignInAsync(request.Username.ToUpper(), request.Password, false, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    var user = await userManager.FindByEmailAsync(request.Username.ToUpper());
                    ArgumentNullException.ThrowIfNull(user);
                    var identity = user as IdentityUser;
                    if (!identity.EmailConfirmed)
                        throw new SystemException("Akun Anda Di Blokir , Silahkan Hubungi Administrator");
                    var roles = await userManager.GetRolesAsync(user);
                    var token = await user.GenerateToken(_appSettings, roles);
                    Profile? profile = null;
                    if (roles.Contains("Teacher"))
                    {
                        profile = dbContext.Teachers.FirstOrDefault(x => x.UserId == identity.Id);
                    }

                    if (roles.Contains("Student"))
                    {
                        profile = dbContext.Students.FirstOrDefault(x => x.UserId == identity.Id);
                    }


                    return Results.Ok(new AuthenticateResponse
                    (user.Name, user.Email, roles, token, profile));
                }

                if (result.IsNotAllowed)
                    throw new SystemException("Akun Anda Di Blokir , Silahkan Hubungi Administrator");
                else
                    throw new SystemException($"Your Account {request.Username} Not Have Access !");
            }
            catch (System.Exception ex)
            {
                return Results.Unauthorized();
            }
        }






    }


    public static class AuthServiceExtention
    {
        public static Task<string> GenerateToken<ApplicationUser>(this ApplicationUser tuser, AppSettings _appSettings, IList<string>? roles = null)
        {

            var user = tuser as IdentityUser;
            var issuer = _appSettings.Issuer;
            var audience = _appSettings.Audience;
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                new Claim("id", user.Id),
                new Claim(JwtRegisteredClaimNames.Name, user.UserName),
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti,
                Guid.NewGuid().ToString())
             }),
                Expires = DateTime.UtcNow.AddDays(7),
                Issuer = issuer,
                Audience = audience,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha512Signature)
            };

            if (roles != null)
            {
                foreach (var item in roles)
                {
                    tokenDescriptor.Subject.AddClaim(new Claim("role", item));
                }
            }
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return Task.FromResult(tokenHandler.WriteToken(token));
        }

    }
}
