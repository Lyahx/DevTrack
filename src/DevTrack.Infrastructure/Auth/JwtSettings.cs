namespace DevTrack.Infrastructure.Auth;

public class JwtSettings
{
    public const string SectionName = "Jwt";

    public string Issuer { get; set; } = "DevTrack";
    public string Audience { get; set; } = "DevTrack";
    public string Secret { get; set; } = string.Empty;
    public int AccessTokenLifetimeDays { get; set; } = 7;
}
