namespace PiketWebApi
{
    public class AppSettings
    {
        public string? Issuer { get; set; } = string.Empty;
        public string? Audience { get; set; } = string.Empty;
        public string? Secret { get; set; } = string.Empty;
        public string? PasswordHashKey { get; set; } = string.Empty;
    }
}
