using Microsoft.AspNetCore.Identity;

namespace PiketWebApi.Data
{
    public class ApplicationUser  :IdentityUser
    {
        public ApplicationUser(string userName) : base(userName)
        {
        }

        public ApplicationUser() { }
        public string? Name { get; set; }
    }
}
