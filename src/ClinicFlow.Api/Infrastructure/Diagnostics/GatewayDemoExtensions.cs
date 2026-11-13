using ClinicFlow.Api.Infrastructure.Http;

namespace ClinicFlow.Api.Infrastructure.Diagnostics;

public static class GatewayDemoExtensions
{
    /// <summary>
    /// GET /api/diagnostics/gateway-status/{code} (Development only): asks the gateway for a status code.
    /// Try 503: the resilience handler retries, and every attempt shows in the logs.
    /// </summary>
    public static IEndpointRouteBuilder MapGatewayDemo(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/diagnostics/gateway-status/{code:int}", async (int code, SmsGatewayClient gateway, CancellationToken ct) =>
                TypedResults.Ok(new { requested = code, gatewayStatus = await gateway.CheckStatusAsync(code, ct) }))
            .WithTags("Diagnostics")
            .WithSummary("Ask the SMS gateway for a status code, to watch the retries in the logs")
            .AllowAnonymous();

        return app;
    }
}
