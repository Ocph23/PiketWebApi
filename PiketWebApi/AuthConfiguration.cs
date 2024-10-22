using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace PiketWebApi
{
    public static class AuthConfiguration
    {
        public static IServiceCollection AddOcphAuthServe(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<AppSettings>(configuration.GetSection("AppSettings"));
            services.Configure<IdentityOptions>(options =>
            {
                // Password settings.
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequireUppercase = true;
                options.Password.RequiredLength = 6;
                options.Password.RequiredUniqueChars = 1;

                // Lockout settings.
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.AllowedForNewUsers = true;

                // User settings.
                options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
                options.User.RequireUniqueEmail = true;
            });
            var key = Encoding.UTF8.GetBytes(configuration["AppSettings:Secret"]!);
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(o =>
            {
                o.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidIssuer = configuration["AppSettings:Issuer"],
                    ValidAudience = configuration["AppSettings:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true
                };

                o.Events = new JwtBearerEvents
                {
                    OnMessageReceived = async context =>
                    {
                        var accessToken = context.Request.Query["access_token"];

                        // If the request is for our hub...
                        var path = context.HttpContext.Request.Path;
                        if (!string.IsNullOrEmpty(accessToken) &&
                            (path.StartsWithSegments("/chathub")))
                        {
                            // Read the token out of the query string
                            context.Token = accessToken;
                            context.Principal = await GetAuthenticationStateAsync(context.Token);
                        }
                    }
                };


            });
            services.AddAuthentication();
            services.AddAuthorization();
            //services.AddControllers();

            return services;
        }

        public static async Task<ClaimsPrincipal> GetAuthenticationStateAsync(string? token)
        {
            if (string.IsNullOrEmpty(token))
                throw new SystemException(); ;

            IdentityModelEventSource.ShowPII = true;
            ClaimsIdentity identity = new ClaimsIdentity();
            var user = new ClaimsPrincipal(identity);
            var state = new AuthenticationState(user);
            try
            {
                ArgumentNullException.ThrowIfNullOrEmpty(nameof(token));
                string secret = "O0ywQA1xv4h1EXL4cnsQ4ReHEoqwMuejtXB4KxOeTCB4nOpV4yVegegbEgtzsZWfl5wlBWBY5kpUPVsEmkVr3V9sxvOPmT4YR3PvUCiw7s1xMrSoCMVqnG7VlSeO469Z";
                string issuer = "https://chatapp.apspapua.com/";
                await Task.Delay(100);
                var tokenHandler = new JwtSecurityTokenHandler();
                SecurityToken validatedToken = null;
                var key = Encoding.ASCII.GetBytes(secret);
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidIssuer = issuer,
                    ValidAudience = issuer,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,

                }, out validatedToken);

                var jwtToken = (JwtSecurityToken)validatedToken;
                string id = jwtToken.Claims.FirstOrDefault(x => x.Type == "id").Value;
                var name = jwtToken.Claims.FirstOrDefault(x => x.Type == "name").Value;
                var sub = jwtToken.Claims.FirstOrDefault(x => x.Type == "sub").Value;
                var email = jwtToken.Claims.FirstOrDefault(x => x.Type == "email").Value;
                var roles = jwtToken.Claims.Where(x => x.Type == "role").ToList();
                var claims = new List<Claim>
                {
                               new Claim(ClaimTypes.NameIdentifier, id.ToString()),
                               new Claim(ClaimTypes.Name, name),
                               new Claim(ClaimTypes.Email, email),
                };

                foreach (var claim in roles)
                {
                    claims.Add(new Claim(ClaimTypes.Role, claim.Value));
                }
                identity = new ClaimsIdentity(claims, "jwt");
                user = new ClaimsPrincipal(identity);
                return user;
            }
            catch (Exception ex)
            {
                return new ClaimsPrincipal();
            }
        }
    }

}
