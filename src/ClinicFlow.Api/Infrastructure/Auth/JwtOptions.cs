namespace ClinicFlow.Api.Infrastructure.Auth;

/// <summary>Settings from the "Jwt" section. The signing key comes from user secrets, never from appsettings.</summary>
public sealed class JwtOptions
{
    public const string SectionName = "Jwt";

    public string Issuer { get; init; } = "";

    public string Audience { get; init; } = "";

    public string SigningKey { get; init; } = "";

    public int ExpiryMinutes { get; init; } = 60;
}
