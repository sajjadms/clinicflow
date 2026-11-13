namespace ClinicFlow.Api.Infrastructure.Diagnostics;

public static class LoggingDemoExtensions
{
    /// <summary>GET /api/diagnostics/error (Development only): throws on purpose to show an Error log.</summary>
    public static IEndpointRouteBuilder MapLoggingDemo(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/diagnostics/error", () =>
            {
                throw new InvalidOperationException("Demo failure: the lab system did not respond.");
            })
            .WithTags("Diagnostics")
            .WithSummary("Throws an exception on purpose, to show how errors are logged")
            .AllowAnonymous();

        return app;
    }
}
