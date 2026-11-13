using System.Net.Http.Headers;
using Microsoft.Extensions.Options;

namespace ClinicFlow.Api.Infrastructure.Http;

public static class HttpClientsExtensions
{
    /// <summary>
    /// Two outside APIs, two base URLs, two typed clients.
    /// IHttpClientFactory creates the HttpClient for each one and reuses the connections safely.
    /// </summary>
    public static IServiceCollection AddExternalApis(this IServiceCollection services, IConfiguration configuration)
    {
        // Settings from appsettings.json, checked when the app starts (a missing base URL fails fast).
        services.AddOptions<ExternalApiOptions>(ExternalApiOptions.Holidays)
            .Bind(configuration.GetSection("ExternalApis:Holidays"))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddOptions<ExternalApiOptions>(ExternalApiOptions.SmsGateway)
            .Bind(configuration.GetSection("ExternalApis:SmsGateway"))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddHttpContextAccessor();
        services.AddTransient<CorrelationIdHandler>();

        // Client 1: public holidays API
        var holidays = services.AddHttpClient<HolidaysClient>((sp, client) =>
            {
                var options = sp.GetRequiredService<IOptionsMonitor<ExternalApiOptions>>().Get(ExternalApiOptions.Holidays);
                client.BaseAddress = new Uri(options.BaseUrl);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.UserAgent.ParseAdd("ClinicFlow/1.0");
            });
        holidays.AddStandardResilienceHandler();               // 1st (outer): retry, timeout, circuit breaker
        holidays.AddHttpMessageHandler<CorrelationIdHandler>(); // 2nd (inner): runs, and logs, once per attempt

        // Client 2: SMS gateway, with its own base URL and an API key header
        var smsGateway = services.AddHttpClient<SmsGatewayClient>((sp, client) =>
            {
                var options = sp.GetRequiredService<IOptionsMonitor<ExternalApiOptions>>().Get(ExternalApiOptions.SmsGateway);
                client.BaseAddress = new Uri(options.BaseUrl);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.UserAgent.ParseAdd("ClinicFlow/1.0");
                if (!string.IsNullOrWhiteSpace(options.ApiKey))
                {
                    client.DefaultRequestHeaders.Add("X-Api-Key", options.ApiKey);   // sent with every request
                }
            });
        smsGateway.AddStandardResilienceHandler();
        smsGateway.AddHttpMessageHandler<CorrelationIdHandler>();

        return services;
    }
}
