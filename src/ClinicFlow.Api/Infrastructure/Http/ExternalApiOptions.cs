using System.ComponentModel.DataAnnotations;

namespace ClinicFlow.Api.Infrastructure.Http;

/// <summary>Settings for one outside API. Read from appsettings.json ("ExternalApis") and checked at startup.</summary>
public sealed class ExternalApiOptions
{
    public const string Holidays = "Holidays";
    public const string SmsGateway = "SmsGateway";

    [Required, Url]
    public string BaseUrl { get; set; } = "";

    /// <summary>Secret: comes from dotnet user-secrets, never from appsettings.json.</summary>
    public string? ApiKey { get; set; }
}
